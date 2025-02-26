using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.BankAccountManagement
{
    public interface ICreateBankAccountService
    {
        Task<Result> CreateBankAccount(Models.Request.Create.BankAccountRegistration bankAccount, CancellationToken cancellationToken);
    }
}
