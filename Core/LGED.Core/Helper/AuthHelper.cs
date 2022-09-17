using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LGED.Core.Helper
{
    public class AuthHelper
    {
         public static JwtSecurityToken GenerateJwtToken(IConfiguration configuration, List<Claim> claims)
        {
            //secret
            var key = Encoding.UTF8.GetBytes(configuration[AppConfigs.InternalIdServer.Secret]);
            var authSigningKey = new SymmetricSecurityKey(key);
            var token = new JwtSecurityToken(
                configuration[AppConfigs.InternalIdServer.ValidIssuer],
                configuration[AppConfigs.InternalIdServer.ValidAudience],
                //allow to use token for 7 days
                //TODO: in production use, set to 1 day only

                expires: DateTime.Now.AddDays(7),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        public static JwtSecurityToken ValidateJwtToken(IConfiguration configuration, string token)
        {
            var key = Encoding.UTF8.GetBytes(configuration[AppConfigs.InternalIdServer.Secret]);
            var authSigningKey = new SymmetricSecurityKey(key);
            var issuer = configuration[AppConfigs.InternalIdServer.ValidIssuer];
            var audience = configuration[AppConfigs.InternalIdServer.ValidAudience];
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = authSigningKey,
                    ClockSkew = TimeSpan.Zero,
                }, out var validatedToken);
                return validatedToken as JwtSecurityToken;
            }
            catch
            {
                return null;
            }
        }
    }
}