using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations.TransactionManagement;
using Services.Interfaces.TransactionManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Test.Transaction
{
    public class CreateTransactionTest
    {
        public readonly ICreateTransactionService _createTransactionService;
        public readonly CancellationToken _cancellationToken = CancellationToken.None;
        public readonly Models.AppContext _context;
        public CreateTransactionTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<Models.AppContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            _context = new Models.AppContext(options);
            SeedData(_context);
            _createTransactionService = new CreateTransactionService(_context, null);
        }
        private void SeedData(Models.AppContext context)
        {
            var bankAccount1 = new Models.BankAccount
            {
                AccountNumber = "1234567890",
                Balance = 1000,
                BankAccountId = Guid.Parse("01954433-7688-78bf-ba16-58f86811e6f1"),
                UserId = Guid.Parse("01954439-7a6a-786a-b28a-c1b71e499f5c")
            };
            var bankAccount2 = new Models.BankAccount
            {
                AccountNumber = "0987654321",
                Balance = 1000,
                BankAccountId = Guid.Parse("01954434-4150-713b-be50-b0f92bd49b92"),
                UserId = Guid.Parse("01954439-a096-732b-bb33-03346fba1769")
            };
            context.BankAccounts.AddRange(bankAccount1, bankAccount2);
            context.SaveChanges();
        }
        [Fact]
        public async Task CreateTransaction_ShouldBeSuccessfulAndLessThanExpectedTime()
        {
            var expectedExecutionTime = 5000;
            var transaction = new Models.Request.Create.TransactionRequest
            {
                BankAccountId = Guid.Parse("01954433-7688-78bf-ba16-58f86811e6f1"),
                RelatedBankAccountId = Guid.Parse("01954434-4150-713b-be50-b0f92bd49b92"),
                Amount = 1000
            };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _createTransactionService.CreateTransaction(transaction, _cancellationToken);
            stopwatch.Stop();
            var executeTime = stopwatch.ElapsedMilliseconds;
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execute time: {executeTime} ms, expected {expectedExecutionTime} ms");
            Assert.True(executeTime < expectedExecutionTime);
            var createdTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.BankAccountId == transaction.BankAccountId);
            Assert.NotNull(createdTransaction);
            Assert.Equal(transaction.BankAccountId, createdTransaction.BankAccountId);
            Trace.WriteLine($"Execution Time: {executeTime} ms");
        }
    }
}
