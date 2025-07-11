﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tenis_pro_back.Models;

namespace tenis_pro_back.Helpers
{
    public class JwtHelper
    {
        private readonly string _secret;

        public JwtHelper(IConfiguration configuration)
        {
            var encryptionHelper = new EncryptionHelper(configuration);
            var encryptedSecret = configuration["JwtSettings:Secret"];
            _secret = encryptionHelper.Decrypt(encryptedSecret);
        }

        public string GenerateToken(User user, int expireMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("userId", user.Id),
                new Claim(ClaimTypes.Name, user.Name + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.ProfileId),
                new Claim("imageUrl", user.Image??"")
            }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null; // Token inválido o expirado
            }
        }

        public string? GetUserIdFromToken(string token)
        {
            var claims = ValidateToken(token);
            return claims?.FindFirst("userId")?.Value;
        }

    }
}
