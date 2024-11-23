namespace IISBackend.BL.Options;

public record FileStorageOptions()
{
    public required string BucketName;

    public required string StorageNamespace;
}