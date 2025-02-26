using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request.Create
{
    public class TransactionRequest
    {
        [Required]
        public Guid BankAccountId { get; set; }
        [Required]
        public Guid RelatedBankAccountId { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
