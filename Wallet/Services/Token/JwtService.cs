using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? configuration["jwt:secretKey"];

            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                      ?? configuration["jwt:issuer"];

            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                        ?? configuration["jwt:audience"];
        }

        public string GenerateToken(ClientsModel client)
        {
            var claims = new[]
            {
            new Claim("id", client.Id.ToString()),
            new Claim("Email", client.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}