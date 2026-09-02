using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.Enums;

/// <summary>
/// Represents the auction status enum.
/// </summary>
public enum AuctionStatus
{
    Draft,
    Scheduled,
    Active,
    Finished,
    Cancelled,
    Unsold
}
