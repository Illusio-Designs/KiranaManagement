using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Application.Stores.Dtos;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Stores;

public class StoreDocumentService : IStoreDocumentService
{
    private const long MaxSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IAppDbContext _db;
    private readonly IFileStorage _files;

    public StoreDocumentService(IAppDbContext db, IFileStorage files)
    {
        _db = db;
        _files = files;
    }

    public async Task<StoreDocumentDto> UploadAsync(
        Guid storeId, DocumentType type, string fileName, string contentType, byte[] content,
        CancellationToken ct = default)
    {
        if (content.Length == 0)
            throw new AppException("Uploaded file is empty.");
        if (content.Length > MaxSizeBytes)
            throw new AppException("File exceeds the 10 MB limit.");

        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == storeId, ct)
                    ?? throw AppException.NotFound("Store not found.");

        // Documents are collected during onboarding, before approval.
        if (store.Status is not (StoreStatus.Pending or StoreStatus.Active))
            throw new AppException($"Documents cannot be uploaded for a {store.Status} store.");

        var safeName = Path.GetFileName(fileName);
        var storagePath = await _files.SaveAsync($"stores/{storeId}", safeName, content, contentType, ct);

        var doc = new StoreDocument
        {
            StoreId = storeId,
            DocumentType = type,
            FileName = safeName,
            ContentType = contentType,
            SizeBytes = content.Length,
            StoragePath = storagePath
        };

        _db.StoreDocuments.Add(doc);
        await _db.SaveChangesAsync(ct);

        return Map(doc);
    }

    public async Task<IReadOnlyList<StoreDocumentDto>> GetForStoreAsync(Guid storeId, CancellationToken ct = default)
    {
        var docs = await _db.StoreDocuments
            .Where(d => d.StoreId == storeId)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(ct);
        return docs.Select(Map).ToList();
    }

    public async Task<(StoreDocumentDto Meta, StoredFile File)> DownloadAsync(Guid documentId, CancellationToken ct = default)
    {
        var doc = await _db.StoreDocuments.FirstOrDefaultAsync(d => d.Id == documentId, ct)
                  ?? throw AppException.NotFound("Document not found.");

        var file = await _files.ReadAsync(doc.StoragePath, ct)
                   ?? throw AppException.NotFound("Stored file is missing.");

        return (Map(doc), file);
    }

    private static StoreDocumentDto Map(StoreDocument d) => new(
        d.Id, d.StoreId, d.DocumentType, d.FileName, d.ContentType, d.SizeBytes, d.CreatedAt);
}
