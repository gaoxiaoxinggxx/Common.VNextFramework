using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Common.VNextFramework.Tools
{
    public class JwtTool
    {
        public static JwtModel Generate(string key, DateTime expiration, Dictionary<string, string> claims)
        {
            if (key.Length < 16) throw new ArgumentOutOfRangeException("The minimum key length is 16");

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var authTime = DateTime.Now;
            var expiresAt = expiration;
            var claimsIdentity = new ClaimsIdentity(claims.AsEnumerable().Select(x => new Claim(x.Key, x.Value)));

            var tokenDescripor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            return new JwtModel
            {
                Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescripor)),
                Expiration = tokenDescripor.Expires
            };
        }
    }

    public class JwtModel
    {
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}