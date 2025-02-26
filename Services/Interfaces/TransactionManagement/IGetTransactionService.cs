using FluentResults;
using Models.Response;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.TransactionManagement
{
    public interface IGetTransactionService
    {
        Task<Result<TransactionResponse>> GetTransactionById(Guid transactionId, CancellationToken cancellationToken);
        Task<Result<PaginatedResponse<TransactionResponse>>> GetTransactions(int page, int pageSize, CancellationToken cancellationToken);
    }
}
