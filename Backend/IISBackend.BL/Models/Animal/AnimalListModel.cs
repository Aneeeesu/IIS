namespace IISBackend.BL.Models.Animal;

public record AnimalListModel : ModelBase
{
    public required string Name { get; set; }
}
