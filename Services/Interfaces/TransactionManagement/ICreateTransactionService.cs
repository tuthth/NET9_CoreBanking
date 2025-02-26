using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.TransactionManagement
{
    public interface ICreateTransactionService
    {
        Task<Result> CreateTransaction(Models.Request.Create.TransactionRequest request, CancellationToken cancellationToken);
    }
}
