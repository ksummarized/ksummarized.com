using api.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UsersDbContext _usersDbContext;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, UsersDbContext usersDbContext, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _usersDbContext = usersDbContext;
            _tokenService = tokenService;
        }

        public async Task<(bool IsSuccess, IEnumerable<string> Error)> Register(UserVM user)
        {
            var userExists = await _userManager.FindByEmailAsync(user.Email);
            if (userExists != null)
            {
                return (false, new[] { "User already exists" });
            }
            var newUser = new UserModel()
            {
                Email = user.Email,
                UserName = user.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                return (true, null);
            }
            else
            {
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }
        }

        public async Task<(bool IsSuccess, AuthResultVM AuthResult, string Error)> Login(UserVM user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                return (false, null, "User not found");
            }
            else if (await _userManager.CheckPasswordAsync(existingUser, user.Password))
            {
                return (true, _tokenService.Generate(user), null);
            }
            else
            {
                return (false, null, "Invalid password");
            }
        }
    }
}
