using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Response;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class GetUserService : BaseService
    {
        public GetUserService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
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
            return Result.Ok(new PaginatedResponse<UserResponse>(users.AsEnumerable(),nextCursor));
        }
    }
}
