using IISBackend.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace IISBackend.BL.Validators
{
    public class RequestValidEnum : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is ScheduleType scheduleType)
            {
                // Check if it is a valid enum
                if (scheduleType != ScheduleType.availableForWalk)
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
