using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.Time;

/// <summary>
/// Represents the i clock interface.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
