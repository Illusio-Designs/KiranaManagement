using Kirana.Application.Stores;
using Kirana.Application.Stores.Dtos;
using Kirana.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>KYC document upload during onboarding (PRD FR-1.2) + Super Admin review.</summary>
[ApiController]
public class StoreDocumentsController : ControllerBase
{
    private readonly IStoreDocumentService _docs;

    public StoreDocumentsController(IStoreDocumentService docs)
    {
        _docs = docs;
    }

    /// <summary>Upload a verification document for a store (allowed while onboarding).</summary>
    [AllowAnonymous]
    [HttpPost("/api/stores/{storeId:guid}/documents")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<StoreDocumentDto>> Upload(
        Guid storeId, [FromForm] DocumentType documentType, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "A non-empty file is required." });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        var dto = await _docs.UploadAsync(
            storeId, documentType, file.FileName, file.ContentType ?? "application/octet-stream",
            ms.ToArray(), ct);

        return Ok(dto);
    }

    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [HttpGet("/api/admin/stores/{storeId:guid}/documents")]
    public async Task<ActionResult<IReadOnlyList<StoreDocumentDto>>> List(Guid storeId, CancellationToken ct)
        => Ok(await _docs.GetForStoreAsync(storeId, ct));

    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [HttpGet("/api/admin/store-documents/{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var (meta, file) = await _docs.DownloadAsync(id, ct);
        return File(file.Content, meta.ContentType, meta.FileName);
    }
}
