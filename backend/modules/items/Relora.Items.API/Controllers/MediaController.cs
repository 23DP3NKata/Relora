using Relora.Shared.Infrastructure.Media;
using Relora.Identity.Infrastructure.Claims;
using Relora.Identity.Infrastructure.Constants;
using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Shared.Domain.Time;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Relora.Items.API.Controllers;

[ApiController]
[Route("api/media")]
/// <summary>
/// Represents the media controller class.
/// </summary>
public sealed class MediaController(
    IMediaUploader mediaUploader,
    IPendingLotMediaUploadRepository pendingUploads,
    IPendingLotProofDocumentUploadRepository pendingProofUploads,
    ILotCatalogRepository catalogRepository,
    ILotRepository lotRepository,
    IClock clock,
    ILogger<MediaController> logger) : ControllerBase
{
    private readonly IMediaUploader _mediaUploader = mediaUploader;
    private readonly IPendingLotMediaUploadRepository _pendingUploads = pendingUploads;
    private readonly IPendingLotProofDocumentUploadRepository _pendingProofUploads = pendingProofUploads;
    private readonly ILotCatalogRepository _catalogRepository = catalogRepository;
    private readonly ILotRepository _lotRepository = lotRepository;
    private readonly IClock _clock = clock;
    private readonly ILogger<MediaController> _logger = logger;

    [HttpPost("upload")]
    [Authorize]
    [RequestSizeLimit(10_000_000)]
    /// <summary>
    /// Performs the upload operation.
    /// </summary>
    /// <param name="file">File.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<ActionResult<string>> Upload([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        if (!IsSupportedImageExtension(file.FileName))
        {
            return BadRequest("Only JPEG, PNG, and WebP images are allowed.");
        }

        await using var stream = file.OpenReadStream();
        if (!HasSupportedImageSignature(stream))
        {
            return BadRequest("The uploaded file is not a valid JPEG, PNG, or WebP image.");
        }

        var ownerId = User.Claims.GetUserId();
        var key = await _mediaUploader.UploadAsync(ownerId, stream, file.FileName, file.ContentType ?? "application/octet-stream");

        try
        {
            var upload = PendingLotMediaUpload.Create(ownerId, key, _clock.UtcNow);
            await _pendingUploads.AddAsync(upload, HttpContext.RequestAborted);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not record pending media upload {MediaKey}.", key);

            try
            {
                await _mediaUploader.DeleteForLotAsync(key);
            }
            catch (Exception cleanupException)
            {
                _logger.LogError(cleanupException, "Could not remove untracked media upload {MediaKey}.", key);
            }

            throw;
        }

        return Ok(new { key });
    }

    [HttpPost("proof-documents/upload")]
    [Authorize]
    [RequestSizeLimit(12_000_000)]
    public async Task<ActionResult> UploadProofDocument([FromForm] IFormFile file, [FromForm] Guid documentTypeId)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        if (file.Length > 10_000_000)
        {
            return BadRequest("Proof document must be 10 MB or smaller.");
        }

        var documentType = await _catalogRepository.GetProofDocumentTypeByIdAsync(
            documentTypeId,
            HttpContext.RequestAborted);

        if (documentType is null || !documentType.IsActive)
        {
            return BadRequest("Proof document type is not available.");
        }

        if (!IsSupportedProofExtension(file.FileName))
        {
            return BadRequest("Only JPEG, PNG, WebP, and PDF proof documents are allowed.");
        }

        await using var stream = file.OpenReadStream();
        if (!HasSupportedProofSignature(stream))
        {
            return BadRequest("The uploaded file is not a valid JPEG, PNG, WebP, or PDF document.");
        }

        var ownerId = User.Claims.GetUserId();
        var contentType = NormalizeProofContentType(file);
        var key = await _mediaUploader.UploadPrivateAsync(ownerId, stream, file.FileName, contentType);

        try
        {
            var upload = PendingLotProofDocumentUpload.Create(
                ownerId,
                documentTypeId,
                file.FileName,
                key,
                contentType,
                file.Length,
                _clock.UtcNow);

            await _pendingProofUploads.AddAsync(upload, HttpContext.RequestAborted);

            return Ok(new
            {
                uploadId = upload.Id,
                upload.DocumentTypeId,
                upload.OriginalFileName,
                upload.MimeType,
                upload.SizeBytes,
                upload.CreatedAtUtc
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not record pending proof document upload.");

            try
            {
                await _mediaUploader.DeleteForLotAsync(key);
            }
            catch (Exception cleanupException)
            {
                _logger.LogError(cleanupException, "Could not remove untracked proof document upload.");
            }

            throw;
        }
    }

    [HttpGet("proof-documents/{lotId:guid}/{documentId:guid}")]
    [Authorize]
    public async Task<ActionResult> GetProofDocument(Guid lotId, Guid documentId)
    {
        var lot = await _lotRepository.GetLotById(lotId, HttpContext.RequestAborted);
        if (lot is null)
        {
            return NotFound();
        }

        var viewerId = User.Claims.GetUserId();
        var viewerIsAdmin = User.IsInRole(Roles.Admin);
        if (!lot.CanShowProofMetadata(viewerId, viewerIsAdmin))
        {
            return Forbid();
        }

        var document = lot.ProofDocuments.FirstOrDefault(item => item.Id == documentId);
        if (document is null)
        {
            return NotFound();
        }

        var url = await _mediaUploader.CreatePrivateReadUrlAsync(
            document.StorageKey,
            TimeSpan.FromMinutes(5),
            HttpContext.RequestAborted);

        return Ok(new
        {
            url,
            expiresInSeconds = 300,
            document.OriginalFileName,
            document.MimeType,
            document.SizeBytes
        });
    }

    [HttpDelete("proof-documents/{lotId:guid}/{documentId:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteProofDocument(Guid lotId, Guid documentId)
    {
        var ownerId = User.Claims.GetUserId();
        var lot = await _lotRepository.GetLotById(lotId, HttpContext.RequestAborted);
        if (lot is null)
        {
            return NotFound();
        }

        var document = lot.RemoveProofDocument(ownerId, documentId);
        await _lotRepository.SaveLotAsync(lot, HttpContext.RequestAborted);

        try
        {
            await _mediaUploader.DeleteForLotAsync(document.StorageKey);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Could not immediately delete proof document {DocumentId}.", document.Id);
        }

        return NoContent();
    }

    [HttpPost("delete")]
    [Authorize]
    public async Task<ActionResult> Delete(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return BadRequest("Key is required.");
        }

        key = key.Trim();
        var ownerId = User.Claims.GetUserId();
        var pendingUpload = await _pendingUploads.GetByOwnerAndKeyAsync(ownerId, key, HttpContext.RequestAborted);

        if (pendingUpload is null)
        {
            return NotFound(new { message = "Temporary media upload was not found." });
        }

        await _pendingUploads.DeleteAsync(pendingUpload, HttpContext.RequestAborted);

        try
        {
            await _mediaUploader.DeleteAsync(ownerId, key);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Could not immediately delete temporary media {MediaKey}. It will be retried by the cleanup worker.",
                key);
        }

        return NoContent();
    }

    private static bool IsSupportedImageExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() is ".jpg" or ".jpeg" or ".png" or ".webp";
    }

    private static bool IsSupportedProofExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() is ".jpg" or ".jpeg" or ".png" or ".webp" or ".pdf";
    }

    private static bool HasSupportedImageSignature(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        var read = stream.Read(header);
        stream.Position = 0;

        return (read >= 3 && header[..3].SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF })) ||
               (read >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) ||
               (read >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8));
    }

    private static bool HasSupportedProofSignature(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        var read = stream.Read(header);
        stream.Position = 0;

        return (read >= 3 && header[..3].SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF })) ||
               (read >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) ||
               (read >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8)) ||
               (read >= 5 && header[..5].SequenceEqual("%PDF-"u8));
    }

    private static string NormalizeProofContentType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType
        };
    }
}
