using api.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IUserService
    {
        Task<(bool IsSuccess,AuthResultVM AuthResult, string Error)> Login(UserVM user);
        Task<(bool IsSuccess, AuthResultVM AuthResult, string Error)> RefreshLogin(TokenRequestDTO tokenRequestDTO);
        Task<(bool IsSuccess, IEnumerable<string> Error)> Register(UserVM user);
    }
}