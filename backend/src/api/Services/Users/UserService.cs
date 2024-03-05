using api.Data;
using api.Data.DTO;
using api.Data.DAO;
using api.Constants;

namespace api.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<(bool isSuccess, string? error)> CreateKeycloakUser(UserDTO user)
    {
        var userInstance = _applicationDbContext.Users.FirstOrDefault(userInstance => userInstance.KeycloakUuid.Equals(user.KeycloakUuid));
        if (userInstance != null)
        {
            return (false, ErrorMessages.UserAlreadyExists);
        }
        var newUser = new UserModel()
        {
            KeycloakUuid = user.KeycloakUuid,
            Email = user.Email
        };
        await _applicationDbContext.Users.AddAsync(newUser);
        await _applicationDbContext.SaveChangesAsync();
        return (true, null);
    }
}
