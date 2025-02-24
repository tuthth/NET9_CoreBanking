using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Models
{
    public class BankAccountValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string accountNumber)
            {
                if (!Regex.IsMatch(accountNumber, @"^\d{9,15}$"))
                {
                    return new ValidationResult("Bank account number must contain only numbers and be between 9 and 15 digits.");
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid bank account number format.");
        }
    }
}
