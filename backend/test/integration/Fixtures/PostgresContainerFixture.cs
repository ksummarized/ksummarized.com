using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using infrastructure.Data;
using Xunit;

namespace Ksummarized.IntegrationTests;

public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private const string Database = "ksummarized_test";
    private const string Username = "postgres";
    private const string Password = "postgres";
    private readonly PostgreSqlContainer _container;

    public string ConnectionString { get; private set; } = string.Empty;

    public PostgresContainerFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:14.0-alpine")
            .WithDatabase(Database)
            .WithUsername(Username)
            .WithPassword(Password)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
        await ApplyMigrationsAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(options)
        {
            TodoLists = null!,
            TodoItems = null!,
            Tags = null!
        };
        await context.Database.MigrateAsync();
    }
}
