using Microsoft.EntityFrameworkCore;

namespace infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<TodoListModel> TodoLists { get; set; }
    public required DbSet<TodoItemModel> TodoItems { get; set; }
    public required DbSet<TagModel> Tags { get; set; }
}
