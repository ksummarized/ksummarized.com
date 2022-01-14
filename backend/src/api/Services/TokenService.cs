using api.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UsersDbContext _usersDbContext;
        private readonly TokenValidationParameters _validationParameters;
        public TokenService(IConfiguration configuration, UsersDbContext usersDbContext, TokenValidationParameters validationParameters)
        {
            _configuration = configuration;
            _usersDbContext = usersDbContext;
            _validationParameters = validationParameters;
        }

        public (AuthResultVM authresult, RefreshToken refreshToken) Generate(UserModel user, RefreshToken refreshToken = null)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                expires: DateTime.UtcNow.AddMinutes(1),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:secret"])),
                    SecurityAlgorithms.HmacSha256
            ));
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            if (refreshToken == null)
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = $"{Guid.NewGuid()}{Guid.NewGuid()}"
                };
            }

            return (new AuthResultVM() { Token = jwt, RefreshToken = refreshToken.Token, ExpiresAt = token.ValidTo }, refreshToken);
        }

        public (bool IsSucess, AuthResultVM AuthResult, string Error) Refresh(TokenRequestDTO tokenRequestDTO, UserModel user, RefreshToken storedToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenIsValid = tokenHandler.ValidateToken(tokenRequestDTO.Token, _validationParameters, out var validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                if (storedToken.IsRevoked || storedToken.DateExpire <= DateTime.UtcNow)
                {
                    return (false, null,  "Refresh token has been revoked or expired");
                }
            }
            return (true, Generate(user, storedToken).authresult, null);
        }
    }
}
