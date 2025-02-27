using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Response;
using Serilog;
using Services.Helpers;
using Services.Interfaces.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class GetUserService : BaseService, IGetUserService
    {
        public GetUserService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        //Get by id
        public async Task<Result<UserResponse>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
            if (user == null)
            {
                Log.Information($"User {userId} is not existed");
                return Result.Fail<UserResponse>("User is not existed");
            }
            Log.Information($"User {userId} is retrieved");
            return Result.Ok(new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            });
        }
        //Get all
        public async Task<Result<IEnumerable<UserResponse>>> GetUsers(CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking().OrderBy(u =>u.CreatedAt).AsQueryable();
            var users = await query.Select(u => new UserResponse
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            }).ToListAsync(cancellationToken);
            Log.Information($"Retrieved {users.Count} users");
            return Result.Ok(users.AsEnumerable());
        }
        //Get with paginated
        public async Task<Result<PaginatedResponse<UserResponse>>> GetUsersPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking().OrderBy(u => u.CreatedAt).AsQueryable();
            var total = await query.CountAsync(cancellationToken);
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(u => new UserResponse
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            }).ToListAsync(cancellationToken);
            var nextCursor = users.LastOrDefault()?.UserId.ToString();
            Log.Information($"Retrieved {users.Count} users");
            return Result.Ok(new PaginatedResponse<UserResponse>(users.AsEnumerable(),nextCursor));
        }
    }
}
