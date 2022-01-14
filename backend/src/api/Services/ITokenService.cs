using api.Data;
using System.Collections.Generic;

namespace api.Services
{
    public interface ITokenService
    {
        (AuthResultVM authresult, RefreshToken refreshToken) Generate(UserModel user, RefreshToken refreshToken = null);
        (bool IsSucess, AuthResultVM AuthResult, string Error) Refresh(TokenRequestDTO tokenRequestDTO, UserModel user, RefreshToken storedToken);
    }
}