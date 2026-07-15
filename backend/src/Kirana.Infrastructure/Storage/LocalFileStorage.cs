using Kirana.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kirana.Infrastructure.Storage;

/// <summary>
/// Saves uploads to the local filesystem under a configurable base path
/// (config key <c>Storage:BasePath</c>, default "uploads"). Swap for blob/S3
/// in production behind the same <see cref="IFileStorage"/> seam.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly string _basePath;

    public LocalFileStorage(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"] ?? "uploads";
    }

    public async Task<string> SaveAsync(string category, string fileName, byte[] content, string contentType,
        CancellationToken ct = default)
    {
        var folder = Path.Combine(_basePath, category);
        Directory.CreateDirectory(folder);

        // Prefix with a GUID to avoid collisions while keeping the original name readable.
        var storedName = $"{Guid.NewGuid():N}_{fileName}";
        var fullPath = Path.Combine(folder, storedName);
        await File.WriteAllBytesAsync(fullPath, content, ct);

        // Return a relative path so it stays portable across environments.
        return Path.Combine(category, storedName).Replace('\\', '/');
    }

    public async Task<StoredFile?> ReadAsync(string storagePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        if (!File.Exists(fullPath)) return null;

        var content = await File.ReadAllBytesAsync(fullPath, ct);
        var contentType = GuessContentType(fullPath);
        return new StoredFile(content, contentType);
    }

    private static string GuessContentType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
