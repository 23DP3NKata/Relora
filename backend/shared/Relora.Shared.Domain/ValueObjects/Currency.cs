using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Shared.Domain.Enums;

namespace Relora.Shared.Domain.ValueObjects;

/// <summary>
/// Represents the currency class.
/// </summary>
public sealed class Currency
{
    /// <summary>
    /// Gets or sets the code used by this type.
    /// </summary>
    public CurrencyType Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Currency"/> class.
    /// </summary>
    /// <param name="code">Code.</param>
    public Currency(CurrencyType code)
    {
        if (!Enum.IsDefined(typeof(CurrencyType), code))
        {
            throw new ArgumentException("Invalid currency code.", nameof(code));
        }

        Code = code;
    }
}
