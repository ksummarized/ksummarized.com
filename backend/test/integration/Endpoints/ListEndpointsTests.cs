using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using contracts.Requests;
using contracts.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Assuming TProgram is Program from api.csproj
public class ListEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;
    private Guid _testUserId;

    private readonly List<int> _createdListIds = new();

    public ListEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _testUserId = Guid.NewGuid();
        _client = _factory.CreateClient();
        // As before, actual authentication setup (e.g., JWT or test auth handler)
        // would be required here or in CustomWebApplicationFactory for these tests to pass auth checks.
        // For now, we proceed assuming the API's ctx.UserId() will effectively be _testUserId for these tests.
    }

    public Task DisposeAsync()
    {
        // Testcontainers in CustomWebApplicationFactory should handle DB cleanup.
        return Task.CompletedTask;
    }

    // Helper to create a list directly for setup if needed (though most tests will use API)
    private async Task<TodoListModel> SeedListAsync(string listName, Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var list = new TodoListModel { Name = listName, Owner = userId, Items = new List<TodoItemModel>() };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();
        _createdListIds.Add(list.Id);
        return list;
    }

    // --- POST /lists (CreateListEndpoint) Tests ---
    [Fact]
    public async Task CreateList_ValidName_Returns201CreatedWithList()
    {
        // Arrange
        var request = new CreateListRequest("My New List");

        // Act
        var response = await _client.PostAsJsonAsync("/lists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdListResponse = await response.Content.ReadFromJsonAsync<GetListResponse>();

        createdListResponse.Should().NotBeNull();
        createdListResponse!.Name.Should().Be(request.Name);
        createdListResponse.Id.Should().NotBe(0);
        createdListResponse.Items.Should().NotBeNull().And.BeEmpty(); // Service creates list with empty items
        _createdListIds.Add(createdListResponse.Id);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be($"/lists/{createdListResponse.Id}");

        // Verify in DB
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbList = await context.TodoLists.FindAsync(createdListResponse.Id);
        dbList.Should().NotBeNull();
        dbList!.Owner.Should().Be(_testUserId); // Assuming service sets owner from HttpContext
    }

    [Fact]
    public async Task CreateList_MissingName_Returns400BadRequest()
    {
        // Arrange
        // CreateListRequest is a record: CreateListRequest(string Name)
        // Sending a JSON that doesn't map to this (e.g. empty or wrong field) might cause issues.
        // For a record with a single string, `null` might not be directly sendable if model validation catches it.
        // ASP.NET Core model binding for records is usually robust.
        // Let's try sending an object that would result in request.Name being null or empty.
        // A common way to test this is to send an empty JSON object, or one with a null name.
        // However, CreateListRequest(null) would be a compile error.
        // The endpoint expects `CreateListRequest request`. If `request.Name` is validated by model binding as required.
        // A more direct way to test model validation for required string:
        var requestWithEmptyName = new { Name = "" }; // Controller might bind this if CreateListRequest was a class
                                                     // For a record `CreateListRequest(string Name)`, this won't bind correctly.
                                                     // The framework will likely return a 400 before it hits the endpoint if Name is non-nullable.
                                                     // If Name can be null in CreateListRequest, then service logic is tested.
                                                     // `CreateListRequest(string Name)` implies Name cannot be null.

        // If CreateListRequest was a class:
        // var request = new CreateListRequest { Name = null }; // This assumes Name is nullable in DTO
        // For a record `CreateListRequest(string Name)`, an empty name is more testable:
        var request = new CreateListRequest("");


        // Act
        var response = await _client.PostAsJsonAsync("/lists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // Further assert on error content if API provides structured validation errors.
    }

    // Test for unauthenticated: POST /lists - Requires client without auth token.
    // [Fact]
    // public async Task CreateList_Unauthenticated_Returns401Unauthorized() { /* ... */ }


    // --- GET /lists/{id} (GetListEndpoint) Tests ---
    [Fact]
    public async Task GetList_ExistingList_Returns200OkWithList()
    {
        // Arrange
        var seededList = await SeedListAsync("List To Get", _testUserId);

        // Act
        var response = await _client.GetAsync($"/lists/{seededList.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var listResponse = await response.Content.ReadFromJsonAsync<GetListResponse>();
        listResponse.Should().NotBeNull();
        listResponse!.Id.Should().Be(seededList.Id);
        listResponse.Name.Should().Be(seededList.Name);
        // Items might be empty or populated based on default GetListOptions in service/endpoint
    }

    [Fact]
    public async Task GetList_NonExistentList_Returns404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/lists/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test for unauthorized access: GET /lists/{id} - Requires creating list as other user.
    // [Fact]
    // public async Task GetList_ListBelongingToAnotherUser_Returns404NotFound() { /* ... */ }

    // Test for unauthenticated: GET /lists/{id}
    // [Fact]
    // public async Task GetList_Unauthenticated_Returns401Unauthorized() { /* ... */ }


    // --- GET /lists (GetAllListsEndpoint) Tests ---
    [Fact]
    public async Task GetAllLists_UserHasLists_Returns200OkWithLists()
    {
        // Arrange
        await SeedListAsync("List Alpha", _testUserId);
        await SeedListAsync("List Beta", _testUserId);

        // Act
        var response = await _client.GetAsync("/lists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lists = await response.Content.ReadFromJsonAsync<List<GetListResponse>>();
        lists.Should().NotBeNull();
        // Count could be more if other tests added lists for the same _testUserId and DB is not fully isolated per test method.
        // For now, check it contains the ones we added.
        lists!.Should().Contain(l => l.Name == "List Alpha");
        lists!.Should().Contain(l => l.Name == "List Beta");
    }

    [Fact]
    public async Task GetAllLists_UserHasNoLists_Returns200OkWithEmptyList()
    {
        // Arrange - Use a new user ID that won't have any lists.
        var newUserClient = _factory.CreateClient(); // Simulate new user client
        // This client needs to be "authenticated" as a new user.
        // This is where a TestAuthHandler shines:
        // newUserClient.DefaultRequestHeaders.Add("X-Test-User-Id", Guid.NewGuid().ToString());

        // For now, to test this scenario without full auth mocking, we'd need to ensure current _testUserId has no lists.
        // This is fragile. A better way is to use a new user ID for this test.
        // Let's assume _client for _testUserId, and we ensure it has no lists for this test.
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userLists = await context.TodoLists.Where(l => l.Owner == _testUserId).ToListAsync();
            context.TodoLists.RemoveRange(userLists);
            await context.SaveChangesAsync();
        }
        _createdListIds.Clear(); // Clear our tracking as we deleted them

        // Act
        var response = await _client.GetAsync("/lists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lists = await response.Content.ReadFromJsonAsync<List<GetListResponse>>();
        lists.Should().NotBeNull().And.BeEmpty();
    }

    // Test for unauthenticated: GET /lists
    // [Fact]
    // public async Task GetAllLists_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // --- PUT /lists/{id} (RenameListEndpoint) Tests ---
    [Fact]
    public async Task RenameList_ExistingList_ValidName_Returns200Ok()
    {
        // Arrange
        var seededList = await SeedListAsync("Old Name", _testUserId);
        var request = new ListRenameRequest { Name = "New Shiny Name" };

        // Act
        var response = await _client.PutAsJsonAsync($"/lists/{seededList.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbList = await context.TodoLists.FindAsync(seededList.Id);
        dbList!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task RenameList_NonExistentList_Returns404NotFound()
    {
        // Arrange
        var request = new ListRenameRequest { Name = "New Name for NonExistent" };

        // Act
        var response = await _client.PutAsJsonAsync("/lists/999999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RenameList_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var seededList = await SeedListAsync("List To Rename With Empty", _testUserId);
        var request = new ListRenameRequest { Name = "" }; // Invalid: Name is required

        // Act
        var response = await _client.PutAsJsonAsync($"/lists/{seededList.Id}", request);

        // Assert
        // This depends on model validation for ListRenameRequest.Name (e.g., [Required], [MinLength(1)])
        // If no validation, service might allow empty name. Assuming validation exists.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Test for unauthenticated: PUT /lists/{id}
    // [Fact]
    // public async Task RenameList_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // Test for unauthorized: PUT /lists/{id} - list belongs to another user
    // [Fact]
    // public async Task RenameList_ListBelongingToAnotherUser_Returns404NotFound() { /* ... */ }


    // --- DELETE /lists/{id} (DeleteListEndpoint) Tests ---
    [Fact]
    public async Task DeleteList_ExistingList_Returns204NoContent()
    {
        // Arrange
        var seededList = await SeedListAsync("List To Delete", _testUserId);

        // Act
        var response = await _client.DeleteAsync($"/lists/{seededList.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbList = await context.TodoLists.FindAsync(seededList.Id);
        dbList.Should().BeNull();
    }

    [Fact]
    public async Task DeleteList_NonExistentList_Returns404NotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/lists/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test for unauthenticated: DELETE /lists/{id}
    // [Fact]
    // public async Task DeleteList_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // Test for unauthorized: DELETE /lists/{id} - list belongs to another user
    // [Fact]
    // public async Task DeleteList_ListBelongingToAnotherUser_Returns404NotFound() { /* ... */ }
}
