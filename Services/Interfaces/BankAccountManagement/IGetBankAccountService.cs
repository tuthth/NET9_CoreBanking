using FluentResults;
using Models.Response;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.BankAccountManagement
{
    public interface IGetBankAccountService
    {
        Task<Result<BankAccountResponse>> GetBankAccountById(Guid bankAccountId, CancellationToken cancellationToken);
        Task<Result<PaginatedResponse<BankAccountResponse>>> GetBankAccountsPagination(int page, int pageSize, CancellationToken cancellationToken);
    }
}
