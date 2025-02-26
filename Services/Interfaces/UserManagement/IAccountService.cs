using FluentResults;
using Models.Request.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.UserManagement
{
    public interface IAccountService
    {
        Task<Result<string>> Login(LoginRequest loginRequest, CancellationToken cancellationToken);
    }
}
