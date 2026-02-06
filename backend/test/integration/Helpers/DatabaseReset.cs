using infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ksummarized.IntegrationTests;

public static class DatabaseReset
{
    public static async Task ResetAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new ApplicationDbContext(options)
        {
            TodoLists = null!,
            TodoItems = null!,
            Tags = null!
        };
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"TagModelTodoItemModel\", \"TodoItems\", \"TodoLists\", \"Tags\" RESTART IDENTITY CASCADE;"
        );
    }
}
