using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.UserManagement
{
    public interface IUpdateUserService
    {
        Task<Result> UpdateUser(Models.Request.Update.UserUpdateRequest user, CancellationToken cancellationToken);
        Task<Result> UpdateUserTransaction(Models.Request.Update.UserUpdateRequest user, CancellationToken cancellationToken);
        Task<Result> RestrictUser(Guid userId, CancellationToken cancellationToken);
        Task<Result> RemoveRestrictForUser(Guid userId, CancellationToken cancellationToken);
    }
}
