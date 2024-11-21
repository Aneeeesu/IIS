using Oci.Common.Retry;
using System.ComponentModel.DataAnnotations;

namespace IISBackend.BL.Validators
{
    public class RoundedToHourAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                // Check if the time is rounded to the nearest hour
                if (dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0)
                {
                    return ValidationResult.Success;
                }

                // Throw exception when validation fails
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be rounded to the nearest hour."
                );
            }

            // Throw exception for invalid data types
            return new ValidationResult($"The {validationContext.DisplayName} field must be a valid DateTime.");
        }
    }
}
