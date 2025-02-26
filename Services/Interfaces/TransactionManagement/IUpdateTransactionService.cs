using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.TransactionManagement
{
    public interface IUpdateTransactionService
    {
        Task<Result> UpdateTransactionType(Guid transactionId, int transactionType, CancellationToken cancellationToken);
        Task<Result> ForceRollbackTransaction(Guid transactionId, CancellationToken cancellationToken);
    }
}
