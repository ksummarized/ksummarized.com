using api.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration) => _configuration = configuration;

        public AuthResultVM Generate(UserVM user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(issuer: _configuration["JWT:issuer"], audience: _configuration["JWT:audience"], expires: DateTime.UtcNow.AddMinutes(5), claims: claims, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:secret"])), SecurityAlgorithms.HmacSha256));
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResultVM() { Token = jwt, ExpiresAt = token.ValidTo };
        }
    }
}
