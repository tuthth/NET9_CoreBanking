using FluentResults;
using Models.Response;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.UserManagement
{
    public interface IGetUserService
    {
        Task<Result<UserResponse>> GetUserById(Guid userId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<UserResponse>>> GetUsers(CancellationToken cancellationToken);
        Task<Result<PaginatedResponse<UserResponse>>> GetUsersPagination(int page, int pageSize, CancellationToken cancellationToken);
    }
}
