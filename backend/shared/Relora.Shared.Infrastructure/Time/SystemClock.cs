using Relora.Shared.Domain.Time;

namespace Relora.Shared.Infrastructure.Time;
/// <summary>
/// Represents the system clock class.
/// </summary>
public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
