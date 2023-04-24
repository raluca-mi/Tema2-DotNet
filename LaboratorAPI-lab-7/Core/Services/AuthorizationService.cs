using DataLayer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class AuthorizationService
    {

        private readonly string _securityKey;

        public AuthorizationService(IConfiguration config)
        {
           _securityKey = config["JWT:SecurityKey"]; // _securityKey = "B?D(G+KbPeShVmYq" ; //
        }

        public string GetToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roleClaim = new Claim("role", user.Role.Name);
            var idClaim = new Claim("userId", user.Id.ToString());
            var infoClaim = new Claim("username", user.Email);

            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Issuer = "Backend",
                Audience = "Frontend",
                Subject = new ClaimsIdentity(new[] { roleClaim, idClaim, infoClaim }),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptior);
            var tokenString = jwtTokenHandler.WriteToken(token);

            return tokenString;
        }

        public bool ValidateToken(string tokenString)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
            };

            if (!jwtTokenHandler.CanReadToken(tokenString.Replace("Bearer ", string.Empty)))
            {
                Console.WriteLine("Invalid Token");
                return false;
            }

            jwtTokenHandler.ValidateToken(tokenString, tokenValidationParameters, out var validatedToken);
            return validatedToken != null;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
 
    }
}
