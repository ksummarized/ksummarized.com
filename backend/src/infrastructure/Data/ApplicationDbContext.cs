using Microsoft.EntityFrameworkCore;

namespace infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoListModel> TodoLists { get; set; }
    public DbSet<TodoItemModel> TodoItems { get; set; }
    public DbSet<TagModel> Tags { get; set; }
}
