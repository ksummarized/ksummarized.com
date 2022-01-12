using api.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

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

        public async Task<bool> Register(UserVM user)
        {
            var userExists = await _userManager.FindByEmailAsync(user.Email);
            if (userExists != null) { return false; }
            var u = new UserModel()
            {
                Email = user.Email,
                UserName = user.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(u, user.Password);
            return result.Succeeded;
        }

        public async Task<AuthResultVM> Login(UserVM user)
        {
            var u = await _userManager.FindByEmailAsync(user.Email);
            if(u == null) { return null; }
            else if( await _userManager.CheckPasswordAsync(u,user.Password)) { return _tokenService.Generate(user); }
            else { return null; }
        }
    }
}
