using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Student_Management.Data;
using Student_Management.DTO.AccountDto;
using Student_Management.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Student_Management.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public TokenService(ApplicationDbContext db, UserManager<User> userManager, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto> IssueTokensAsync(User user)
        {
            var accessMinutes = int.Parse(_configuration["JWT:AccessTokenMinutes"] ?? "5");
            var refreshMinutes = int.Parse(_configuration["JWT:RefreshTokenMinutes"] ?? "5");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "superSecretKey@345"));
            var accessExpiresAt = DateTime.UtcNow.AddMinutes(accessMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: accessExpiresAt,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshExpiresAt = DateTime.UtcNow.AddMinutes(refreshMinutes);
            var refreshToken = new RefreshToken
            {
                Token = GenerateSecureRandomString(),
                UserId = user.Id,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshExpiresAt
            };

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = accessExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshExpiresAt
            };
        }

        public async Task<TokenResponseDto?> RefreshAsync(string refreshToken)
        {
            var existing = await _db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);
            if (existing == null || !existing.IsActive)
                return null;

            existing.RevokedAtUtc = DateTime.UtcNow;

            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null)
            {
                await _db.SaveChangesAsync();
                return null;
            }

            return await IssueTokensAsync(user);
        }

        private static string GenerateSecureRandomString(int byteLength = 64)
        {
            var bytes = RandomNumberGenerator.GetBytes(byteLength);
            return Convert.ToBase64String(bytes);
        }
    }
}
