using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Response;
using Serilog;
using Services.Helpers;
using Services.Interfaces.BankAccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.BankAccountManagement
{
    public class GetBankAccountService : BaseService, IGetBankAccountService
    {
        public GetBankAccountService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result<BankAccountResponse>> GetBankAccountById(Guid bankAccountId, CancellationToken cancellationToken)
        {
            var bankAccount = await _context.BankAccounts.AsNoTracking().FirstOrDefaultAsync(b => b.BankAccountId == bankAccountId, cancellationToken);
            if (bankAccount == null)
            {
                Log.Information($"Bank account {bankAccountId} is not existed");
                return Result.Fail<BankAccountResponse>("Bank account is not existed");
            }
            Log.Information($"Bank account {bankAccountId} is retrieved");
            return Result.Ok(new BankAccountResponse
            {
                BankAccountId = bankAccount.BankAccountId,
                AccountNumber = bankAccount.AccountNumber,
                Balance = bankAccount.Balance,
                UserId = bankAccount.UserId,
                CreatedAt = bankAccount.CreatedAt
            });
        }
        public async Task<Result<PaginatedResponse<BankAccountResponse>>> GetBankAccountsPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.BankAccounts.AsNoTracking().OrderBy(b => b.CreatedAt).AsQueryable();
            var total = await query.CountAsync(cancellationToken);
            var bankAccounts = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(b => new BankAccountResponse
            {
                BankAccountId = b.BankAccountId,
                AccountNumber = b.AccountNumber,
                Balance = b.Balance,
                UserId = b.UserId,
                CreatedAt = b.CreatedAt
            }).ToListAsync(cancellationToken);
            var nextCursor = bankAccounts.LastOrDefault()?.BankAccountId.ToString();
            Log.Information($"Retrieved {bankAccounts.Count} bank accounts");
            return Result.Ok(new PaginatedResponse<BankAccountResponse>(bankAccounts.AsEnumerable(), nextCursor));
        }
    }
}
