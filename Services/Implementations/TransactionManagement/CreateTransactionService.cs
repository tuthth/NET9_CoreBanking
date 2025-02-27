using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using Services.Interfaces.TransactionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.TransactionManagement
{
    public class CreateTransactionService : BaseService, ICreateTransactionService
    {
        public CreateTransactionService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> CreateTransaction(Models.Request.Create.TransactionRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var isBankAccountExist = await _context.BankAccounts.AnyAsync(x => x.BankAccountId == request.BankAccountId, cancellationToken);
                if (!isBankAccountExist)
                {
                    Log.Information($"Bank account {request.BankAccountId} not found");
                    return Result.Fail(new Error("Bank account not found"));
                }
                var isRelatedBankAccountExist = await _context.BankAccounts.AnyAsync(x => x.BankAccountId == request.RelatedBankAccountId, cancellationToken);
                if (!isRelatedBankAccountExist)
                {
                    Log.Information($"Related bank account {request.RelatedBankAccountId} not found");
                    return Result.Fail(new Error("Related bank account not found"));
                }
                var isBankAccountEqualRelatedBankAccount = request.BankAccountId == request.RelatedBankAccountId;
                if (isBankAccountEqualRelatedBankAccount)
                {
                    Log.Information($"Bank account and related bank account cannot be the same");
                    return Result.Fail(new Error("Bank account and related bank account cannot be the same"));
                }
                var newTransaction = new Models.Transaction
                {
                    BankAccountId = request.BankAccountId,
                    RelatedBankAccountId = request.RelatedBankAccountId,
                    TransactionType = (Int32)TransactionType.Progressing,
                    Amount = request.Amount
                };
                await _context.Transactions.AddAsync(newTransaction, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                Log.Information($"Transaction {newTransaction.TransactionId} is created");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(ex, $"Error when creating transaction");
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}
