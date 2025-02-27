using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models.Response;
using Serilog;
using Services.Helpers;
using Services.Interfaces.TransactionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.TransactionManagement
{
    public class GetTransactionService : BaseService, IGetTransactionService
    {
        public GetTransactionService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result<TransactionResponse>> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.TransactionId == transactionId, cancellationToken);
            if (transaction == null)
            {
                Log.Information($"Transaction {transactionId} not found");
                return Result.Fail<TransactionResponse>(new Error("Transaction not found"));
            }
            return Result.Ok(new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                BankAccountId = transaction.BankAccountId,
                RelatedBankAccountId = transaction.RelatedBankAccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt
            });
        }
        public async Task<Result<PaginatedResponse<TransactionResponse>>> GetTransactions(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Transactions.AsNoTracking().OrderBy(x => x.CreatedAt).AsQueryable();
            var total = await query.CountAsync(cancellationToken);
            var transactions = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new TransactionResponse
            {
                TransactionId = x.TransactionId,
                BankAccountId = x.BankAccountId,
                RelatedBankAccountId = x.RelatedBankAccountId,
                TransactionType = x.TransactionType,
                Amount = x.Amount,
                CreatedAt = x.CreatedAt
            }).ToListAsync(cancellationToken);
            var nextCursor = transactions.LastOrDefault()?.TransactionId.ToString();
            Log.Information($"Retrieved {transactions.Count} transactions");
            return Result.Ok(new PaginatedResponse<TransactionResponse>(transactions.AsEnumerable(), nextCursor));

        }
    }
}
