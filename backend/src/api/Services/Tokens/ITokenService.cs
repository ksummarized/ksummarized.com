using api.Data.DAO;
using api.Data.DTO;

namespace api.Services.Tokens;

public interface ITokenService
{
    (AuthResultDTO authresult, RefreshToken refreshToken) Generate(UserModel user, RefreshToken? refreshToken = null);
    (bool IsSuccess, AuthResultDTO? AuthResult, string? Error) Refresh(TokenRequestDTO tokenRequestDTO, UserModel user, RefreshToken storedToken);
}
