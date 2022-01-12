using api.Data;

namespace api.Services
{
    public interface ITokenService
    {
        AuthResultVM Generate(UserVM user);
    }
}