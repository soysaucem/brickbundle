using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace BrickBundle.WebAPI
{
    public static class Authentication
    {
        /// <summary>
        /// Generates a new JwtSecurityToken for the specified user.
        /// </summary>
        public static string GenerateToken(long userID, string username, int expireMinutes = 1440)
        {
            var claims = new[]
            {
                new Claim("userID", userID.ToString()),
                new Claim("username", username)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.JwtSigningKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var myToken = new JwtSecurityToken(
                issuer: Env.JwtIssuer,
                audience: Env.JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(myToken);
        }

        /// <summary>
        /// Retrieves indentifying information of an authenticated API user.
        /// </summary>
        public class ApiIdentity
        {
            public ApiIdentity(IIdentity claimsIdentity)
            {
                var myClaimsIdentity = claimsIdentity as ClaimsIdentity;
                UserID = long.Parse(myClaimsIdentity.FindFirst("userID").Value);
                Username = myClaimsIdentity.FindFirst("username").Value;
            }

            public long UserID { get; set; }
            public string Username { get; set; }
        }
    }
}
