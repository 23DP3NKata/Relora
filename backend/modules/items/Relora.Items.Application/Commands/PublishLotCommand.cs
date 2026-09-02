using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Relora.Items.Application.Commands;

/// <summary>
/// Represents the publish lot command record.
/// </summary>
public record PublishLotCommand(Guid LotId, Guid SellerId) : IRequest<Guid>;
