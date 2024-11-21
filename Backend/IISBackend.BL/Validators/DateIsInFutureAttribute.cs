using System.ComponentModel.DataAnnotations;

namespace IISBackend.BL.Validators
{
    public class DateIsInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                // Check if the date is in the future
                if (dateTime > DateTime.Now)
                {
                    return ValidationResult.Success;
                }

                // Throw exception when validation fails
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be in the future."
                );
            }

            // Throw exception for invalid data types
            return new ValidationResult($"The {validationContext.DisplayName} field must be a valid DateTime.");
        }
    }
}
