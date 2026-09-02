using Relora.Support.Domain;

namespace Relora.Support.Application.Interfaces;

public interface ISupportRequestRepository
{
    Task AddAsync(SupportRequest supportRequest, CancellationToken cancellationToken);
}
