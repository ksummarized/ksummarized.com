using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ksummarized.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly PostgresContainerFixture Fixture;
    protected WebApplication App = null!;
    protected HttpClient Client = null!;
    protected readonly Guid UserId = Guid.NewGuid();

    protected IntegrationTestBase(PostgresContainerFixture fixture)
    {
        Fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await DatabaseReset.ResetAsync(Fixture.ConnectionString);

        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:KSummarized"] = Fixture.ConnectionString,
            ["KeycloakJwt:Issuer"] = "https://test-issuer",
            ["KeycloakJwt:Audience"] = "test-audience",
            ["KeycloakJwt:Secret"] = TestJwtToken.PublicKey
        };

        App = Program.BuildApp(
            Array.Empty<string>(),
            builder =>
            {
                builder.Environment.EnvironmentName = "Test";
                builder.WebHost.ConfigureKestrel(options => options.Listen(System.Net.IPAddress.Loopback, 0));
                builder.Configuration.AddInMemoryCollection(settings);
            },
            services =>
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                        options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
            });

        await App.StartAsync();

        var server = App.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();
        var address = addresses?.Addresses.FirstOrDefault();
        if (address is null)
        {
            throw new InvalidOperationException("Could not determine server address for integration tests.");
        }

        Client = new HttpClient { BaseAddress = new Uri(address) };
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtToken.Create(UserId));
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        if (App is not null)
        {
            await App.StopAsync();
            await App.DisposeAsync();
        }
    }
}
