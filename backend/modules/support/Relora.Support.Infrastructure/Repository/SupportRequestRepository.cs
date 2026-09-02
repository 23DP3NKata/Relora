using Relora.Persistance;
using Relora.Support.Application.Interfaces;
using Relora.Support.Domain;

namespace Relora.Support.Infrastructure.Repository;

public sealed class SupportRequestRepository(ReloraDbContext dbContext) : ISupportRequestRepository
{
    public async Task AddAsync(SupportRequest supportRequest, CancellationToken cancellationToken)
    {
        await dbContext.SupportRequests.AddAsync(supportRequest, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
