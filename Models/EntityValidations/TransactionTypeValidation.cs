using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Models
{
    public class TransactionTypeValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int transactionType)
            {
                if (transactionType > 4 || transactionType < 1)
                    return new ValidationResult("Invalid transaction type");
            }

            return ValidationResult.Success;
        }
    }
}
