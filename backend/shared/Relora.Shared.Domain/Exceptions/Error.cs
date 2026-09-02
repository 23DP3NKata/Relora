using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.Exceptions;

/// <summary>
/// Represents the error class.
/// </summary>
public class Error : System.Exception
{
    /// <summary>
    /// Gets or sets the status code used by this type.
    /// </summary>
    public int StatusCode { get; }

    public Error(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}
