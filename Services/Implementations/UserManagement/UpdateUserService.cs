﻿using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class UpdateUserService : BaseService, IUpdateUserService
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
            if (existingUser.IsRestricted)
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
        public async Task<Result> UpdateUserTransaction(Models.Request.Update.UserUpdateRequest user, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId, cancellationToken);
                    if (existingUser == null)
                    {
                        return Result.Fail("User is not existed");
                    }
                    if (existingUser.IsRestricted)
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
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(ex.Message);
                }
            }
        }
        public async Task<Result> RestrictUser(Guid userId, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
                    if (existingUser == null)
                    {
                        return Result.Fail("User is not existed");
                    }
                    if (existingUser.IsRestricted)
                    {
                        return Result.Fail($"User is restricted to {existingUser.RestrictedExpiredAt}");
                    }
                    existingUser.IsRestricted = true;
                    existingUser.RestrictedExpiredAt = DateTime.Now.AddMonths(1);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(ex.Message);
                }
            }
        }
        public async Task<Result> RemoveRestrictForUser(Guid userId, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
                    if (existingUser == null)
                    {
                        return Result.Fail("User is not existed");
                    }
                    if (!existingUser.IsRestricted)
                    {
                        return Result.Fail("User is not restricted");
                    }
                    if(existingUser.RestrictedExpiredAt > DateTime.UtcNow)
                    {
                        return Result.Fail($"User is restricted to {existingUser.RestrictedExpiredAt}");
                    }
                    existingUser.IsRestricted = false;
                    existingUser.RestrictedExpiredAt = null;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(ex.Message);
                }
            }
        }
    }
}
