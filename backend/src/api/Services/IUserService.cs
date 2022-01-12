using api.Data;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IUserService
    {
        Task<bool> Register(UserVM user);
    }
}