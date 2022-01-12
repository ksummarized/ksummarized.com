using api.Data;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IUserService
    {
        Task<AuthResultVM> Login(UserVM user);
        Task<bool> Register(UserVM user);
    }
}