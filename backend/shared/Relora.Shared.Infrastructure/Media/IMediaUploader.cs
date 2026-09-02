using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Shared.Infrastructure.Media;

namespace Relora.Shared.Infrastructure.Media;
/// <summary>
/// Represents the i media uploader interface.
/// </summary>
public interface IMediaUploader
{
    Task<string> UploadAsync(Guid ownerId, Stream stream, string fileName, string contentType);
    Task<string> UploadPrivateAsync(Guid ownerId, Stream stream, string fileName, string contentType);
    Task<string> CreatePrivateReadUrlAsync(string key, TimeSpan lifetime, CancellationToken cancellationToken);
    Task DeleteAsync(Guid ownerId, string key);
    Task DeleteForLotAsync(string key);
    Task<IReadOnlyList<string>> GetKeysOlderThanAsync(string prefix, DateTime cutoffUtc, CancellationToken cancellationToken);
}
