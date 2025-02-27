using AutoMapper;
using FluentResults;
using Models;
using Models.Request.Create;
using Serilog;
using Services.Interfaces.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class CreateUserService : BaseService, ICreateUserService
    {
        public CreateUserService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> CreateNewUser(Models.Request.Create.UserRegistration user, CancellationToken cancellationToken)
        {
            var newUser = new User
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = _appExtension.CreateHashPassword(user.Password),
                IsRestricted = false
            };
            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync();
            Log.Information($"User {newUser.Username} is created");
            return Result.Ok();
        }
        public async Task<Result> CreateNewUserTransaction(Models.Request.Create.UserRegistration user, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var newUser = new User
                    {
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role,
                        PasswordHash = _appExtension.CreateHashPassword(user.Password),
                        IsRestricted = false
                    };
                    await _context.Users.AddAsync(newUser, cancellationToken);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(cancellationToken);
                    Log.Information($"User {newUser.Username} is created");
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    Log.Error(ex, $"Error when creating user {user.Username}");
                    return Result.Fail(ex.Message);
                }
            }
        }
    }
}
