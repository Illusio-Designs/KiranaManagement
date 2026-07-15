using Kirana.Application.Common.Interfaces;
using Kirana.Application.Stores.Dtos;
using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Stores;

public interface IStoreDocumentService
{
    /// <summary>Uploads a KYC document for a store during onboarding.</summary>
    Task<StoreDocumentDto> UploadAsync(
        Guid storeId, DocumentType type, string fileName, string contentType, byte[] content,
        CancellationToken ct = default);

    Task<IReadOnlyList<StoreDocumentDto>> GetForStoreAsync(Guid storeId, CancellationToken ct = default);

    /// <summary>Returns the stored file bytes for download (Super Admin review).</summary>
    Task<(StoreDocumentDto Meta, StoredFile File)> DownloadAsync(Guid documentId, CancellationToken ct = default);
}
