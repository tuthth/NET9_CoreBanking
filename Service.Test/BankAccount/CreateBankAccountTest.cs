using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations.BankAccountManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Test.BankAccount
{
    public class CreateBankAccountTest
    {
        public readonly CreateBankAccountService _createBankAccountService;
        public readonly CancellationToken _cancellationToken = CancellationToken.None;
        public readonly Models.AppContext _context;
        public CreateBankAccountTest()
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
            _createBankAccountService = new CreateBankAccountService(_context, null);
        }
        private void SeedData(Models.AppContext context)
        {
            var user = new Models.User
            {
                UserId = Guid.Parse("019544f8-1923-711c-acd3-b48c75c65e39"),
                Email = "test@gmail.com",
                PasswordHash = "12345678",
                Username = "Test User",
                Role = 1,
                IsRestricted = false
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
        [Fact]
        public async Task CreateBankAccount_ShouldBeSuccessfulAndLessThanExpectedTime()
        {
            var expectedExecutionTime = 5000;
            var bankAccount = new Models.Request.Create.BankAccountRegistration
            {
                AccountNumber = "123456789",
                UserId = Guid.Parse("019544f8-1923-711c-acd3-b48c75c65e39")
            };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _createBankAccountService.CreateBankAccount(bankAccount, _cancellationToken);
            stopwatch.Stop();
            var executeTime = stopwatch.ElapsedMilliseconds;
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execute time: {executeTime} ms, expected {expectedExecutionTime} ms");
            Assert.True(executeTime < expectedExecutionTime);
            var createdBankAccount = await _context.BankAccounts.FirstOrDefaultAsync(u => u.AccountNumber == bankAccount.AccountNumber);
            Assert.NotNull(createdBankAccount);
            Assert.Equal(bankAccount.AccountNumber, createdBankAccount.AccountNumber);
            Trace.WriteLine($"Execution Time: {executeTime} ms");
        }
    }
}
