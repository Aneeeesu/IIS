namespace IISBackend.BL.Models;

public record AnimalListModel : ModelBase
{
    public required string Name { get; set; }
}
