using IISBackend.BL.Models.Interfaces;
using IISBackend.BL.Validators;

namespace IISBackend.BL.Models.User;

public record UserBaseModel : IModel
{
    public Guid Id { get; init; }
    [StringNotEmpty(ErrorMessage = "First name cannot be empty")]
    public required string FirstName { get; set; }
    [StringNotEmpty(ErrorMessage = "Last name cannot be empty")]
    public required string LastName { get; set; }
}