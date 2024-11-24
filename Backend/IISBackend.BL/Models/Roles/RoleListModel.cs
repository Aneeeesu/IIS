using IISBackend.BL.Models.Interfaces;

namespace IISBackend.BL.Models.Roles;

public record RoleListModel
{
    public required string Name { get; set; }
}
