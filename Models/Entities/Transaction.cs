using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Transaction
    {
        [Required]
        public Guid TransactionId { get; set; } = Guid.CreateVersion7();
        [Required]
        public Guid BankAccountId { get; set; }
        [Required]
        public Guid RelatedBankAccountId { get; set; }
        [Required]
        [TransactionTypeValidation]
        public int TransactionType { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual BankAccount? BankAccountNavigation { get; set; }
        public virtual BankAccount? RelatedAccountNavigation { get; set; }
    }
}
