﻿using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces.BankAccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.BankAccountManagement
{
    public class CreateBankAccountService : BaseService, ICreateBankAccountService
    {
        public CreateBankAccountService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> CreateBankAccount(Models.Request.Create.BankAccountRegistration bankAccount, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var isUserExisted = await _context.Users.AnyAsync(u => u.UserId == bankAccount.UserId, cancellationToken);
                    if (!isUserExisted)
                    {
                        return Result.Fail("User is not existed");
                    }
                    var isBankAccountExisted = await _context.BankAccounts.AnyAsync(b => b.AccountNumber == bankAccount.AccountNumber, cancellationToken);
                    if (isBankAccountExisted)
                    {
                        return Result.Fail("Bank account is already existed, pleased choose another number");
                    }
                    var newBankAccount = new Models.BankAccount
                    {
                        AccountNumber = bankAccount.AccountNumber,
                        UserId = bankAccount.UserId
                    };
                    await _context.BankAccounts.AddAsync(newBankAccount, cancellationToken);
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
