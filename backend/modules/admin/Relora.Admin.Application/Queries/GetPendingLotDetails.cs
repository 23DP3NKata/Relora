using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetPendingLotDetails(Guid LotId, Guid adminId) : IRequest<PendingLotPreviewDetailsDto?>;
