using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Relora.Shared.Infrastructure.Media;
/// <summary>
/// Represents the media uploader class.
/// </summary>
public sealed class MediaUploader(IOptions<MediaOptions> options, IAmazonS3 s3) : IMediaUploader
{
    private readonly IAmazonS3 _s3 = s3;
    private readonly MediaOptions _mediaOptions = options.Value;

    /// <summary>
    /// Performs the upload async operation.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="contentType">Content type.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<string> UploadAsync(Guid ownerId, Stream stream, string fileName, string contentType)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Media owner id is required.");
        }

        var extension = Path.GetExtension(fileName);
        var key = _mediaOptions.BuildKey(ownerId, Guid.NewGuid(), extension);

        var request = new PutObjectRequest
        {
            BucketName = _mediaOptions.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType,
            DisablePayloadSigning = true, // for public access
            DisableDefaultChecksumValidation = true // for public access
        };

        request.Headers.CacheControl = "public, max-age=31536000"; // chache for 1 year


        await _s3.PutObjectAsync(request);

        return key;
    }

    public async Task<string> UploadPrivateAsync(Guid ownerId, Stream stream, string fileName, string contentType)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Media owner id is required.");
        }

        var extension = Path.GetExtension(fileName);
        var key = $"proof-origin/{ownerId:N}/{Guid.NewGuid():N}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = _mediaOptions.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };

        request.Metadata["privacy"] = "proof-of-origin";
        request.Headers.CacheControl = "private, max-age=0, no-store";

        await _s3.PutObjectAsync(request);

        return key;
    }

    public Task<string> CreatePrivateReadUrlAsync(string key, TimeSpan lifetime, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key) ||
            key.Contains("..", StringComparison.Ordinal) ||
            key.StartsWith("/", StringComparison.Ordinal) ||
            !key.StartsWith("proof-origin/", StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Invalid private media key.");
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _mediaOptions.BucketName,
            Key = key.Trim(),
            Expires = DateTime.UtcNow.Add(lifetime)
        };

        return Task.FromResult(_s3.GetPreSignedURL(request));
    }

    public async Task DeleteAsync(Guid ownerId, string key)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Media owner id is required.");
        }

        var ownerPrefix = $"lots/{ownerId:N}/";
        if (!key.StartsWith(ownerPrefix, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("You do not own this media file.");
        }

        await DeleteForLotAsync(key);
    }

    public async Task DeleteForLotAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _mediaOptions.BucketName,
            Key = key
        };

        await _s3.DeleteObjectAsync(request);
    }

    public async Task<IReadOnlyList<string>> GetKeysOlderThanAsync(
        string prefix,
        DateTime cutoffUtc,
        CancellationToken cancellationToken)
    {
        var keys = new List<string>();
        string? continuationToken = null;

        do
        {
            var response = await _s3.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = _mediaOptions.BucketName,
                Prefix = prefix,
                ContinuationToken = continuationToken
            }, cancellationToken);

            keys.AddRange(response.S3Objects
                .Where(item => item.LastModified is { } lastModified && lastModified.ToUniversalTime() <= cutoffUtc)
                .Select(item => item.Key));

            continuationToken = response.IsTruncated == true
                ? response.NextContinuationToken
                : null;
        }
        while (!string.IsNullOrWhiteSpace(continuationToken));

        return keys;
    }
}
