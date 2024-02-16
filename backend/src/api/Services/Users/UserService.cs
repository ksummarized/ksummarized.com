using api.Data;
using api.Data.DTO;
using api.Data.DAO;
using api.Constants;

namespace api.Services.Users;

public class UserService : IUserService
{
    private readonly UsersDbContext _usersDbContext;

    public UserService(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<(bool isSuccess, string? error)> CreateKeycloakUser(UserDto user)
    {
        var userInstance = _usersDbContext.Users.FirstOrDefault(userInstance => userInstance.KeycloakUuid.Equals(user.KeycloakUuid));
        if (userInstance != null)
        {
            return (false, ErrorMessages.UserAlreadyExists);
        }
        var newUser = new UserModel()
        {
            KeycloakUuid = user.KeycloakUuid,
            Email = user.Email
        };
        await _usersDbContext.Users.AddAsync(newUser);
        await _usersDbContext.SaveChangesAsync();
        return (true, null);
    }
}
