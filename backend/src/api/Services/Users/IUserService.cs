using api.Data.DTO;

namespace api.Services.Users;

public interface IUserService
{
    Task<(bool isSuccess, string? error)> CreateKeycloakUser(UserDto user);
}
