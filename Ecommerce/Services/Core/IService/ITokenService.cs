using Ecommerce.Data.Entities;
using System.Security.Claims;

namespace Ecommerce.Services.Core.IService
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<Token?> GetOne(string token, string userId);
        Task UpdateAsync(Token token, string newRefreshToken);
        Task<Boolean> DeleteOneAsync(string refreshtToken, string userId);
        Task CreateAsync(string refreshToken, string userId, DateTime refreshTokenExpiryTime);
        Task DeleteManyAsync(string userId);
    }
}