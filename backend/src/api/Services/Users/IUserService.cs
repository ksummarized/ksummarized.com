using api.Data.DTO;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Services.Users;

public interface IUserService
{
    Task<(bool IsSuccess, AuthResultDTO AuthResult, string Error)> Login(UserDTO user);
    Task<(bool IsSuccess, AuthResultDTO AuthResult, string Error)> RefreshLogin(TokenRequestDTO tokenRequestDTO);
    Task<(bool IsSuccess, IEnumerable<string> Error)> Register(UserDTO user);
    Task Logout(string username);
}
