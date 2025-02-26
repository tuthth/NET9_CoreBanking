using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.UserManagement
{
    public interface ICreateUserService
    {
        Task<Result> CreateNewUser(Models.Request.Create.UserRegistration user, CancellationToken cancellationToken);
        Task<Result> CreateNewUserTransaction(Models.Request.Create.UserRegistration user, CancellationToken cancellationToken);
    }
}
