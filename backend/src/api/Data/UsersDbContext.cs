using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class UsersDbContext : IdentityDbContext<UserModel>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
