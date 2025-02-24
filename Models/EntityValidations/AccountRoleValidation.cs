using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Models
{
    public class AccountRoleValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int role)
            {
                if (role != 1 && role!= 2) // Only digits, 9-15 characters
                {
                    return new ValidationResult("Invalid role.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
