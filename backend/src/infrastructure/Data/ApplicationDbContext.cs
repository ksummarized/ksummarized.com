using Microsoft.EntityFrameworkCore;

namespace infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<TodoListModel> TodoLists { get; set; }
}
