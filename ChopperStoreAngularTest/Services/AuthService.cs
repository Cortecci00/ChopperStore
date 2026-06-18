using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChopperStoreAngularTest.Models;
using Microsoft.IdentityModel.Tokens;

namespace ChopperStoreAngularTest.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.email ?? ""),
                new(ClaimTypes.Name, user.username ?? ""),
                new("isAdmin", user.isAdmin.ToString().ToLowerInvariant())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationMinutes = double.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "120");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
