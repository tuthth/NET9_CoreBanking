using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class TransactionResponse
    {
        public Guid TransactionId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid RelatedBankAccountId { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
