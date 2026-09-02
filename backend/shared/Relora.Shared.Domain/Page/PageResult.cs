using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.Page;
public sealed class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}
