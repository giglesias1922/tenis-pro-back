using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public string GenerateToken(string userId, string username, string role, int expireMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("userId", userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
