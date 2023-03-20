using api.Data;
using api.Data.DAO;
using api.Data.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Services.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _validationParameters;
        public TokenService(JwtOptions jwtOptions, TokenValidationParameters validationParameters)
        {
            _jwtOptions = jwtOptions;
            _validationParameters = validationParameters;
        }

        public (AuthResultDTO authresult, RefreshToken refreshToken) Generate(UserModel user, RefreshToken? refreshToken = null)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret!)),
                    SecurityAlgorithms.HmacSha256
            ));
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            refreshToken ??= new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = $"{Guid.NewGuid()}{Guid.NewGuid()}"
                };

            return (new AuthResultDTO() { Token = jwt, RefreshToken = refreshToken.Token, ExpiresAt = token.ValidTo }, refreshToken);
        }

        public (bool IsSuccess, AuthResultDTO? AuthResult, string? Error) Refresh(TokenRequestDTO tokenRequestDTO, UserModel user, RefreshToken storedToken)
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
                    return (false, null, "Refresh token has been revoked or expired");
                }
            }
            return (true, Generate(user, storedToken).authresult, null);
        }
    }
}
