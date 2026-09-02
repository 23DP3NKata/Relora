using Relora.Shared.Domain.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Relora.Persistance;

public sealed class TransactionRunner(ReloraDbContext context) : ITransactionRunner
{
    private readonly ReloraDbContext _context = context;

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
