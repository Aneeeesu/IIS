namespace IISBackend.BL.Models;

public record LoginModel
{
    public required string Name { get; set; }
    public required string Password { get; set; }
}
