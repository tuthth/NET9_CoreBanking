using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class UpdateUserService : BaseService
    {
        public UpdateUserService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> UpdateUser(Models.Request.Update.UserUpdateRequest user, CancellationToken cancellationToken)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId, cancellationToken);
            if (existingUser == null)
            {
                return Result.Fail("User is not existed");
            }
            if(existingUser.IsRestricted)
            {
                return Result.Fail($"User is restricted to {existingUser.RestrictedExpiredAt}");
            }
            if (!string.IsNullOrEmpty(user.Username))
            {
                existingUser.Username = user.Username;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.PasswordHash = _appExtension.CreateHashPassword(user.Password);
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser.Email = user.Email;
            }
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
