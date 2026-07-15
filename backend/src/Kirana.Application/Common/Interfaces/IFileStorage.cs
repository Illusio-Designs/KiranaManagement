namespace Kirana.Application.Common.Interfaces;

public record StoredFile(byte[] Content, string ContentType);

/// <summary>Stores and retrieves uploaded files (local disk in dev; blob/S3 later).</summary>
public interface IFileStorage
{
    /// <summary>Saves the content and returns an opaque storage path/key.</summary>
    Task<string> SaveAsync(string category, string fileName, byte[] content, string contentType,
        CancellationToken ct = default);

    Task<StoredFile?> ReadAsync(string storagePath, CancellationToken ct = default);
}
