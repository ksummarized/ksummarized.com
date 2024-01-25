using api.Data.DAO.Identity;
using api.Data.DAO.ToDo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationDbContext : IdentityDbContext<UserModel>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ToDoList> ToDoLists { get; set; }
    public DbSet<ToDoItem> ToDoItems { get; set; }
    public DbSet<ToDoTag> ToDoTags { get; set; }
    public DbSet<ToDoItemTags> ToDoItemTags {get; set;}
}
