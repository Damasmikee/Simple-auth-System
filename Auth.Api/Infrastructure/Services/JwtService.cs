using AuthOnion.Api.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthOnion.Api.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        private DateTime _expiry;

        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            _expiry = DateTime.UtcNow.AddMinutes(_settings.DurationMinutes);
        }

        public DateTime GetExpiry() => _expiry;

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("username", user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: _expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
