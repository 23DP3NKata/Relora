using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Identity.Domain;

namespace Relora.Identity.Application.Interfaces;
public interface IRefreshTokenRepository
{
    Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    Task AddRefreshToken(RefreshToken refreshToken);
    Task DeleteRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
}
