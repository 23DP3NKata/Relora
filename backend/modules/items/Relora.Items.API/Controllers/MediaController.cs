using Relora.Shared.Infrastructure.Media;
using Relora.Identity.Infrastructure.Claims;
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
    IClock clock,
    ILogger<MediaController> logger) : ControllerBase
{
    private readonly IMediaUploader _mediaUploader = mediaUploader;
    private readonly IPendingLotMediaUploadRepository _pendingUploads = pendingUploads;
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

    private static bool HasSupportedImageSignature(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        var read = stream.Read(header);
        stream.Position = 0;

        return (read >= 3 && header[..3].SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF })) ||
               (read >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) ||
               (read >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8));
    }
}
