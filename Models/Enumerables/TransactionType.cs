using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum TransactionType
    {
        Successed = 1,
        Progressing = 2,
        Cancelled = 3,
        Rollbacked = 4
    }
    //4: request from police for preventing scam
}
