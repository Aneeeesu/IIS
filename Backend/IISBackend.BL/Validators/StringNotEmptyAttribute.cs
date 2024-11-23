using System.ComponentModel.DataAnnotations;

namespace IISBackend.BL.Validators
{
    public class StringNotEmptyAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                // Check if the string is not empty
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    return ValidationResult.Success;
                }

                // Throw exception when validation fails
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must not be empty."
                );
            }

            // Throw exception for invalid data types
            return new ValidationResult($"The {validationContext.DisplayName} field must be a valid string.");
        }
    }
}
