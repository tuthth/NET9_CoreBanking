using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BankAccount
    {
        [Required]
        public Guid BankAccountId { get; set; } = Guid.CreateVersion7();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [BankAccountValidation]
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual User? UserNavigation { get; set; }
    }
}
