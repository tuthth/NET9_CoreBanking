using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        [Required]
        public Guid UserId { get; set; } = Guid.CreateVersion7();
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [AccountRoleValidation]
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRestricted { get; set; }
        public DateTime? RestrictedExpiredAt { get;set; }
        public virtual ICollection<BankAccount>? BankAccountsNavigation { get; set; }
    }
}
