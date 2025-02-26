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
    public class UpdateUserTest
    {
        private readonly UpdateUserService _updateUserService;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;
        private readonly Models.AppContext _context;
        public UpdateUserTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<Models.AppContext>()
                .UseInMemoryDatabase(databaseName: "UpdateUserTest")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new Models.AppContext(options);
            SeedDatabase(_context);
            _updateUserService = new UpdateUserService(_context, null);
        }
        private void SeedDatabase(Models.AppContext context)
        {
            var normalUser = new Models.User
            {
                UserId = Guid.CreateVersion7(),
                Username = "User 1",
                Email = "user1@gmail.com",
                PasswordHash = "password",
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                IsRestricted = false,
                RestrictedExpiredAt = null
            };
            var restrictedUser = new Models.User
            {
                UserId = Guid.CreateVersion7(),
                Username = "User 2",
                Email = "user2@gmail.com",
                PasswordHash = "password",
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                IsRestricted = true,
                RestrictedExpiredAt = DateTime.UtcNow.AddDays(14)
            };
            var normalUser2 = new Models.User
            {
                UserId = Guid.CreateVersion7(),
                Username = "User 3",
                Email = "user3@gmail.com",
                PasswordHash = "password",
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                IsRestricted = false,
                RestrictedExpiredAt = null
            };
            context.Users.AddRange(normalUser, restrictedUser, normalUser2);
            context.SaveChanges();
        }
        [Fact]
        public async Task UpdateUser_ShouldBeSuccessful()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var user = new Models.Request.Update.UserUpdateRequest
            {
                UserId = _context.Users.FirstOrDefault(u => u.Username == "User 1").UserId,
                Username = "User 1 Updated",
                Email = "test1@gmail.com",
                Password = "password1"
            };
            var result = await _updateUserService.UpdateUser(user, _cancellationToken);
            stopwatch.Stop();
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
        }
        [Fact]
        public async Task UpdateUser_ShouldFail_WhenUserIsRestricted()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var user = new Models.Request.Update.UserUpdateRequest
            {
                UserId = _context.Users.FirstOrDefault(u => u.Username == "User 2").UserId,
                Username = "User 2 Updated",
                Email = "test2@gmail.com",
                Password = "password2"
            };
            var result = await _updateUserService.UpdateUser(user, _cancellationToken);
            stopwatch.Stop();
            Assert.True(result.IsFailed);
            Trace.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
        [Fact]
        public async Task UpdateUserTransaction_ShouldBeSuccessful()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var user = new Models.Request.Update.UserUpdateRequest
            {
                UserId = _context.Users.FirstOrDefault(u => u.Username == "User 1").UserId,
                Username = "User 1 Updated",
                Email = "test1@gmail.com",
                Password = "password1"
            };
            var result = await _updateUserService.UpdateUserTransaction(user, _cancellationToken);
            stopwatch.Stop();
            Assert.True(result.IsSuccess);
            Trace.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
        [Fact]
        public async Task CompareUserUpdate_NormalAndTransaction()
        {
            var user1 = new Models.Request.Update.UserUpdateRequest
            {
                UserId = _context.Users.FirstOrDefault(u => u.Username == "User 1").UserId,
                Username = "User 1 Updated",
                Email = "test1@gmail.com",
                Password = "password1"
            };
            var user3 = new Models.Request.Update.UserUpdateRequest
            {
                UserId = _context.Users.FirstOrDefault(u => u.Username == "User 3").UserId,
                Username = "User 3 Updated",
                Email = "test3@gmail.com",
                Password = "password3"
            };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result1 = await _updateUserService.UpdateUser(user1, _cancellationToken);
            stopwatch.Stop();
            Assert.True(result1.IsSuccess);
            var normalExecutionTime = stopwatch.ElapsedMilliseconds;
            Trace.WriteLine($"Normal Update Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
            var result2 = await _updateUserService.UpdateUserTransaction(user3, _cancellationToken);
            stopwatch.Stop();
            Assert.True(result2.IsSuccess);
            var transactionExecutionTime = stopwatch.ElapsedMilliseconds;
            Trace.WriteLine($"Transaction Update Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            Assert.False(normalExecutionTime < transactionExecutionTime);
            stopwatch.Restart();
            var updatedUser1 = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user1.UserId);
            var updatedUser2 = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user3.UserId);
            Assert.NotNull(updatedUser1);
            Assert.NotNull(updatedUser2);
        }
    }
}
