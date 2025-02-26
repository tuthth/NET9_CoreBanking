using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations.UserManagement;
using Xunit;

namespace Service.Test.User
{
    public class GetUsersTest
    {
        private readonly GetUserService _getUserService;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;
        private readonly Models.AppContext _context;

        public GetUsersTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<Models.AppContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new Models.AppContext(options);
            SeedDatabase(_context);
            _getUserService = new GetUserService(_context, null);
        }

        private void SeedDatabase(Models.AppContext context)
        {
            var users = Enumerable.Range(1, 1000).Select(i => new Models.User
            {
                UserId = Guid.CreateVersion7(),
                Username = $"User {i}",
                Email = $"user{i}@test.com",
                PasswordHash = "password",
                Role = 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                IsRestricted = false,
                RestrictedExpiredAt = null
            }).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetUsers_ShouldBeFasterThan_Paginated()
        {
            // Measure full retrieval
            var stopwatch = Stopwatch.StartNew();
            var allUsersResult = await _getUserService.GetUsers(_cancellationToken);
            stopwatch.Stop();
            var allUsersTime = stopwatch.ElapsedMilliseconds;

            // Measure paginated retrieval (first page)
            stopwatch.Restart();
            var paginatedResult = await _getUserService.GetUsersPagination(1, 1000, _cancellationToken);
            stopwatch.Stop();
            var paginatedTime = stopwatch.ElapsedMilliseconds;

            // Assert results are valid
            Assert.True(allUsersResult.IsSuccess);
            Assert.True(paginatedResult.IsSuccess);
            Assert.NotEmpty(allUsersResult.Value);
            Assert.NotEmpty(paginatedResult.Value.Data);

            // Log the times
            Trace.WriteLine($"Full Retrieval Time: {allUsersTime} ms");
            Trace.WriteLine($"Paginated Retrieval Time: {paginatedTime} ms");

            // Ensure pagination is faster
            Assert.True(paginatedTime < allUsersTime);
        }
    }
}