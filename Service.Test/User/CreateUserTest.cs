using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations.UserManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Test.User
{
    public class CreateUserTest
    {
        private readonly CreateUserService _createUserService;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;
        private readonly Models.AppContext _context;
        public CreateUserTest()
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
            _createUserService = new CreateUserService(_context, null);
        }
        [Fact]
        public async Task CreateUser_ShouldBeSuccessfulAndLessThanExpectedTime()
        {
            var expectedExecutionTime = 500;
            var user = new Models.Request.Create.UserRegistration
            {
                Username = "User 1",
                Email = "test@gmail.com",
                Password = "password",
                Role = 1
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _createUserService.CreateNewUser(user, _cancellationToken);
            stopwatch.Stop();

            var executeTime = stopwatch.ElapsedMilliseconds;
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execute time: {executeTime} ms, expected {expectedExecutionTime} ms");
            Assert.True(executeTime < expectedExecutionTime);
            var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            Assert.NotNull(createdUser);
            Assert.Equal(user.Username, createdUser.Username);

            Trace.WriteLine($"Execution Time: {executeTime} ms");
        }
        [Fact]
        public async Task CreateUserTransaction_ShouldBeSuccessfulAndLessThanExpectedTime()
        {
            var expectedExecutionTime = 500;
            var user = new Models.Request.Create.UserRegistration
            {
                Username = "User 1",
                Email = "test@gmail.com",
                Password = "password",
                Role = 1
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _createUserService.CreateNewUserTransaction(user, _cancellationToken);
            stopwatch.Stop();

            var executeTime = stopwatch.ElapsedMilliseconds;
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execute time: {executeTime} ms, expected {expectedExecutionTime} ms");
            Assert.True(executeTime < expectedExecutionTime);
            var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            Assert.NotNull(createdUser);
            Assert.Equal(user.Username, createdUser.Username);

            Trace.WriteLine($"Execution Time: {executeTime} ms");
        }
        [Fact]
        public async Task CompareNormalAndTransactionCreate()
        {
            var expectedExecutionTime = 500;
            var user1 = new Models.Request.Create.UserRegistration
            {
                Username = "User 1",
                Email = "test@gmail.com",
                Password = "password",
                Role = 1
            };
            var user2 = new Models.Request.Create.UserRegistration
            {
                Username = "User 2",
                Email = "test2@gmail.com",
                Password = "password",
                Role = 1
            };

            var stopwatch = Stopwatch.StartNew();
            var resultNormal = await _createUserService.CreateNewUser(user1, _cancellationToken);
            stopwatch.Stop();
            var normalExecutionTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            var resultTransaction = await _createUserService.CreateNewUserTransaction(user2, _cancellationToken);
            stopwatch.Stop();
            var transactionExecutionTime = stopwatch.ElapsedMilliseconds;

          
            Assert.True(resultNormal.IsSuccess);
            Assert.True(resultTransaction.IsSuccess);
            var createdUser1 = await _context.Users.FirstOrDefaultAsync(u => u.Username == user1.Username);
            var createdUser2 = await _context.Users.FirstOrDefaultAsync(u => u.Username == user2.Username);

            Assert.NotNull(createdUser1);
            Assert.NotNull(createdUser2);
            Assert.Equal(user1.Username, createdUser1.Username);
            Assert.Equal(user2.Username, createdUser2.Username);

            Trace.WriteLine($"Normal Create Execution Time: {normalExecutionTime} ms");
            Trace.WriteLine($"Transaction Create Execution Time: {transactionExecutionTime} ms");

            Assert.True(normalExecutionTime < expectedExecutionTime,
                $"Normal create took {normalExecutionTime} ms, expected < {expectedExecutionTime} ms");

            Assert.True(transactionExecutionTime < expectedExecutionTime * 2,
                $"Transaction create took {transactionExecutionTime} ms, expected < {expectedExecutionTime * 2} ms");
        }
    }
}
