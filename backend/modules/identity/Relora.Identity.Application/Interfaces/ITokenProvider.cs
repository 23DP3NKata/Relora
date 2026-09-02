using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Identity.Application.Interfaces;

/// <summary>
/// Represents the i token provider interface.
/// </summary>
public interface ITokenProvider
{
    string GenerateAccessToken(Domain.User user);
    string GenerateRefreshToken();
}
