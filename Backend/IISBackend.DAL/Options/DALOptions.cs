namespace IISBackend.DAL.Options;
public record DALOptions
{
    public required string ConnectionString { get; init; }
    public bool TestEnvironment { get; init; } = false;
    public bool RecreateDatabase { get; init; } = false;
}
