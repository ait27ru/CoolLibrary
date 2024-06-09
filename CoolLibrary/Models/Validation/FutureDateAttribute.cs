using System;
using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Models.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            if (currentValue.HasValue && currentValue.Value.Date > DateTime.Now.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Date2 must not be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
