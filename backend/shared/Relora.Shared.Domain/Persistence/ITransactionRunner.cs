namespace Relora.Shared.Domain.Persistence;

public interface ITransactionRunner
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);
}
