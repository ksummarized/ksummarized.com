using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Assuming TProgram is the Program class from your API project.
// If your API's Program class is in a specific namespace, e.g., 'MyApi',
// you would use 'CustomWebApplicationFactory<MyApi.Program>'.
// If it's in the global namespace of the api project, 'Program' might suffice.
// Let's assume 'Program' from the referenced 'api.csproj' is the entry point.
// This might require adding a using directive for the API's namespace if 'Program' is namespaced,
// or that the 'Program' class is made public or InternalsVisibleTo is set for the test project.
// For now, we'll use 'Program' as a placeholder for TProgram.
// The actual TProgram needs to be the main entry point class of the web application project (api.csproj).
// If api.csproj has a public partial class Program {}, that should work.
// If not, we might need to find the actual class used with WebApplication.CreateBuilder.

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory<Program>> // Replace 'Program' if needed
{
    private readonly CustomWebApplicationFactory<Program> _factory; // Replace 'Program' if needed

    public HealthCheckTests(CustomWebApplicationFactory<Program> factory) // Replace 'Program' if needed
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_RootPath_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        response.StatusCode.Should().Be(HttpStatusCode.OK); // Or NotFound, depending on what root path does

        // For a more robust health check, you'd typically hit a specific /healthz endpoint
        // and check its content. This is a basic check that the server starts and responds.
    }

    [Fact]
    public async Task Get_NonExistentPath_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/this-path-should-not-exist-ever");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// Note: The 'Program' type used as TProgram here is a placeholder.
// It must be resolvable to the main class in your backend/src/api/api.csproj.
// If that class is, for example, 'public class Program { ... }' or 'public partial class Program { ... }'
// in the global namespace of the 'api' project, this might work.
// If it's internal, InternalsVisibleTo might be needed in api.csproj:
// <ItemGroup>
//   <InternalsVisibleTo Include="integration" />
// </ItemGroup>
// Or, if Program is in a namespace like 'MyApiNamespace', then use CustomWebApplicationFactory<MyApiNamespace.Program>.
// I will assume for now that 'Program' is accessible. The build step will confirm.
