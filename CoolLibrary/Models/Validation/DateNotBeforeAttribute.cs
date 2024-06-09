using System;
using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Models.Validation
{
    public class DateNotBeforeAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateNotBeforeAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonProperty == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime?)comparisonProperty.GetValue(validationContext.ObjectInstance);

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue.Value.Date < comparisonValue.Value.Date)
            {
                return new ValidationResult(ErrorMessage ?? "date2 cannot be earlier than date1.");
            }
            return ValidationResult.Success;
        }
    }
}
