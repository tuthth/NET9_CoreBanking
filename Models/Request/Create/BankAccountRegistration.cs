using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request.Create
{
    public class BankAccountRegistration
    {
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
    }
}
