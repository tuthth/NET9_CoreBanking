using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces.TransactionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.TransactionManagement
{
    public class UpdateTransactionService : BaseService, IUpdateTransactionService
    {
        public UpdateTransactionService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result> UpdateTransactionType(Guid transactionId, int transactionType, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var transactionEntity = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId, cancellationToken);
                if (transactionEntity == null)
                {
                    return Result.Fail(new Error("Transaction not found"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Rollbacked)
                {
                    return Result.Fail(new Error("Transaction type is forced rollback, cannot change"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Cancelled)
                {
                    return Result.Fail(new Error("Transaction type is cancelled, cannot change"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Successed)
                {
                    return Result.Fail(new Error("Transaction type is successed, cannot change"));
                }
                transactionEntity.TransactionType = transactionType;
                await _context.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result.Fail(new Error(ex.Message));
            }
        }
        //Force rollback successful transaction
        public async Task<Result> ForceRollbackTransaction(Guid transactionId, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var transactionEntity = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId, cancellationToken);
                if (transactionEntity == null)
                {
                    return Result.Fail(new Error("Transaction not found"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Rollbacked)
                {
                    return Result.Fail(new Error("Transaction type is forced rollback, cannot change"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Cancelled)
                {
                    return Result.Fail(new Error("Transaction type is cancelled, cannot change"));
                }
                if (transactionEntity.TransactionType == (Int32)TransactionType.Progressing)
                {
                    return Result.Fail(new Error("Transaction type is progressing, cannot force rollback"));
                }
                transactionEntity.TransactionType = (Int32)TransactionType.Rollbacked;
                await _context.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}
