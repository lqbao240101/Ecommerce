using AutoMapper;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.Services.Core.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Services.Core.Catalog
{
    public class TokenService : EntityBaseService<Token>, ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration) : base(context, mapper)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
        public async Task<Token?> GetOne(string token, string userId)
        {
            var item = await _context.Tokens.FirstOrDefaultAsync(e => e.RefreshToken.Equals(token) && e.UserId.Equals(userId));
            return item;
        }
        public async Task UpdateAsync(Token token, string newRefreshToken)
        {
            token.RefreshToken = newRefreshToken;
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();
        }
        public async Task<Boolean> DeleteOneAsync(string refreshToken, string userId)
        {
            var item = await _context.Tokens.FirstOrDefaultAsync(e => e.RefreshToken.Equals(refreshToken) && e.UserId.Equals(userId));
            if (item is null)
                return false;
            else
            {
                _context.Tokens.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public async Task CreateAsync(string refreshToken, string userId, DateTime refreshTokenExpiryTime)
        {
            var item = new Token()
            {
                RefreshToken = refreshToken,
                UserId = userId,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            };
            await _context.Tokens.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteManyAsync(string userId)
        {
            var items = await _context.Tokens.Where(e => e.UserId.Equals(userId)).ToListAsync();
            if (items.Count != 0)
            {
                _context.Tokens.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}