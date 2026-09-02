using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Relora.Shared.Domain.Abstractions;
/// <summary>
/// Represents the i domain event interface.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}
