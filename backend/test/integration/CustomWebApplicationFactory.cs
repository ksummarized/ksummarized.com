using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using System.Threading.Tasks;
using infrastructure.Data; // For ApplicationDbContext

// Assuming TProgram is the Program class from your API project.
// If your API's Program class is in a specific namespace, you might need to add:
// using MyApiNamespace;
// Or if it's just 'Program', ensure it's accessible.
// For this example, let's assume the API's Program class is accessible as 'Program'
// or via a fully qualified name if necessary. The referenced api.csproj should make its Program discoverable.

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine") // Using a specific version
        .WithDatabase("test_db")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the original DbContext registration
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(ApplicationDbContext));

            // Add DbContext using Testcontainers connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Optional: Add any other test-specific service overrides here
        });

        builder.UseEnvironment("Test"); // Set environment to Test
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Apply migrations
        // Create a scope to get services
        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

            // It's good practice to ensure the database is created and migrations are applied.
            // For a newly created container, EnsureCreated() might be enough if not using migrations,
            // but ApplyMigrations is more robust for a production-like setup.
            await dbContext.Database.MigrateAsync();
        }
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync(); // Explicitly dispose the container
        await base.DisposeAsync();
    }
}
