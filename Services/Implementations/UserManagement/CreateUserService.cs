using AutoMapper;
using FluentResults;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class CreateUserService : BaseService
    {
        public CreateUserService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> CreateNewUser(Models.Request.Create.UserRegistration user, CancellationToken cancellationToken)
        {
            var newUser = _mapper.Map<Models.User>(user);
            newUser.PasswordHash = _appExtension.CreateHashPassword(user.Password);
            newUser.IsRestricted = false;
            newUser.RestrictedExpiredAt = null;
            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
