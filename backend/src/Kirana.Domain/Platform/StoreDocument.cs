using Kirana.Domain.Common;
using Kirana.Domain.Common.Enums;

namespace Kirana.Domain.Platform;

/// <summary>
/// A KYC / verification document uploaded for a store during onboarding.
/// Platform-level (carries StoreId as a plain FK) so documents can be uploaded
/// before the owner can log in and reviewed by the Super Admin.
/// </summary>
public class StoreDocument : BaseEntity
{
    public Guid StoreId { get; set; }

    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
}
