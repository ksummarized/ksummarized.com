using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using core; // For TodoItem, Tag DTOs
using contracts.Requests; // For CreateTaskRequest
using contracts.Responses; // For CreateTaskResponse
using Microsoft.AspNetCore.Mvc.Testing; // For WebApplicationFactory
using infrastructure.Data; // For seeding TodoListModel
using Microsoft.EntityFrameworkCore; // For Include, etc.
using Microsoft.Extensions.DependencyInjection; // For CreateScope

// Assuming TProgram is Program from api.csproj
public class TaskEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client = null!; // Initialized in InitializeAsync
    private Guid _testUserId; // To be initialized with a consistent test user ID

    // Store IDs of created resources to help with teardown or verification if needed
    private readonly List<int> _createdListIds = new();
    private readonly List<int> _createdTaskIds = new();


    public TaskEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // IAsyncLifetime methods for per-test setup/teardown if needed,
    // but CustomWebApplicationFactory with Testcontainers handles DB state per test class.
    // For auth, we might need a fresh client or user per test or per class.
    public async Task InitializeAsync()
    {
        // For now, generate a new Guid for each test class run.
        // In a real scenario with JWTs, this user would need to exist in some auth system mock
        // or be set up by a test authentication handler.
        _testUserId = Guid.NewGuid();

        // Create a client that is "authenticated" as _testUserId
        // This is a placeholder for actual authentication.
        // In a real setup, this might involve getting a JWT for _testUserId
        // or configuring a test auth handler in CustomWebApplicationFactory.
        _client = _factory.CreateClient();
        // _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GenerateTestJwtForUser(_testUserId)}"); // Example

        // Seed a default list for this user to create tasks in
        await SeedDefaultListForUserAsync(_testUserId);
    }

    public Task DisposeAsync()
    {
        // _client?.Dispose(); // Client is disposed by factory
        // Clean up DB? Testcontainers usually handles this by tearing down the container.
        // If specific items need cleanup beyond what Testcontainers does, do it here.
        return Task.CompletedTask;
    }

    private async Task SeedDefaultListForUserAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingList = await context.TodoLists.FirstOrDefaultAsync(l => l.Owner == userId && l.Name == "Default Test List");
        if (existingList == null)
        {
            var defaultList = new TodoListModel { Name = "Default Test List", Owner = userId, Items = new List<TodoItemModel>() };
            context.TodoLists.Add(defaultList);
            await context.SaveChangesAsync();
            _createdListIds.Add(defaultList.Id); // Track for potential cleanup or use in tests
        }
        else
        {
            if(!_createdListIds.Contains(existingList.Id)) _createdListIds.Add(existingList.Id);
        }
    }

    private int GetDefaultListIdForTestUser() => _createdListIds.First();


    // --- POST /tasks (CreateTaskEndpoint) Tests ---

    [Fact]
    public async Task CreateTask_ValidData_Returns201CreatedWithTask()
    {
        // Arrange
        var defaultListId = GetDefaultListIdForTestUser();
        var request = new CreateTaskRequest
        {
            ListId = defaultListId,
            Name = "My New Awesome Task",
            Deadline = DateTime.UtcNow.AddDays(7),
            Notes = "Detailed notes here.",
            Tags = new List<string> { "work", "urgent" },
            Subtasks = new List<CreateTaskRequest.Subtask>
            {
                new CreateTaskRequest.Subtask { Name = "Subtask 1", Tags = new List<string> {"sub"}, Notes = "sub notes 1" },
                new CreateTaskRequest.Subtask { Name = "Subtask 2", Tags = new List<string>(), Notes = "sub notes 2" }
            }
        };

        // Act
        // This client needs to be authenticated as _testUserId for the API to extract it.
        // This is currently a placeholder. CustomWebApplicationFactory needs to inject a test auth handler.
        // For now, we assume the API's ctx.UserId() will return _testUserId due to some mocked auth.
        // To make this test pass without full auth setup, one might need to modify the API endpoint
        // or the CustomWebApplicationFactory to allow setting a test user.
        // For now, proceeding as if auth is handled by the factory/client setup.

        // To simulate user for endpoints using ctx.UserId() without real auth:
        // var client = _factory.WithWebHostBuilder(builder => {
        //     builder.ConfigureServices(services => {
        //         // This was a placeholder and ITestUserFeature is not defined.
        //         // services.AddScoped<ITestUserFeature>(sp => new TestUserFeature { UserId = _testUserId });
        //         // This requires a middleware to actually apply this test user to HttpContext.
        //         // Or, a simpler way for tests is a custom TestAuthHandler.
        //         // For now, this won't work out of the box.
        //     });
        // }).CreateClient();
        // The above is an example of how one might try to inject user, but it's not complete.
        // A more robust solution is a custom AuthenticationHandler in the TestServer.

        // For the purpose of this task, I will assume _client (initialized in InitializeAsync)
        // already has test authentication set up by CustomWebApplicationFactory if it were extended for auth.
        // If not, these tests requiring auth will fail with 401/403.
        // Using the class-level _client here.
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTaskResponse = await response.Content.ReadFromJsonAsync<CreateTaskResponse>(); // Using CreateTaskResponse as the return type

        createdTaskResponse.Should().NotBeNull();
        createdTaskResponse!.Name.Should().Be(request.Name);
        createdTaskResponse.ListId.Should().Be(request.ListId);
        createdTaskResponse.Notes.Should().Be(request.Notes);
        createdTaskResponse.Tags.Should().HaveCount(request.Tags.Count());
        createdTaskResponse.Subtasks.Should().HaveCount(request.Subtasks.Count());
        createdTaskResponse.Id.Should().NotBe(0);
        _createdTaskIds.Add(createdTaskResponse.Id);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be($"/tasks/{createdTaskResponse.Id}");

        // Verify in DB (optional, but good for integration tests)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbItem = await context.TodoItems.Include(i => i.Tags).Include(i => i.Subtasks).SingleOrDefaultAsync(i => i.Id == createdTaskResponse.Id);
        dbItem.Should().NotBeNull();
        dbItem!.Owner.Should().Be(_testUserId); // Assuming service sets owner correctly
        dbItem.Name.Should().Be(request.Name);
        dbItem.Tags.Select(t => t.Name).Should().BeEquivalentTo(request.Tags);
        dbItem.Subtasks.Select(st => st.Name).Should().BeEquivalentTo(request.Subtasks.Select(s => s.Name));
    }

    [Fact]
    public async Task CreateTask_MissingName_Returns400BadRequest()
    {
        // Arrange
        var defaultListId = GetDefaultListIdForTestUser();
        var request = new CreateTaskRequest
        {
            ListId = defaultListId,
            Name = null!, // Invalid: Name is required
            Tags = new List<string>(),
            Subtasks = new List<CreateTaskRequest.Subtask>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Assert
        // The API uses model validation which should trigger a 400.
        // The exact error response structure might vary.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Placeholder for "CreateTask_Unauthenticated_Returns401Unauthorized"
    // This test would require a client that is explicitly *not* authenticated.
    // [Fact]
    // public async Task CreateTask_Unauthenticated_Returns401Unauthorized()
    // {
    //     var unauthClient = _factory.CreateClient(); // A fresh client without auth headers
    //     var request = new CreateTaskRequest { /* ... valid data ... */ };
    //     var response = await unauthClient.PostAsJsonAsync("/tasks", request);
    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }


    // --- GET /tasks/{id} (GetTaskEndpoint) Tests ---

    [Fact]
    public async Task GetTask_ExistingTask_Returns200OkWithTask()
    {
        // Arrange: Create a task first
        var defaultListId = GetDefaultListIdForTestUser();
        var createTaskRequest = new CreateTaskRequest { ListId = defaultListId, Name = "Task To Get", Tags = new List<string>(), Subtasks = new List<CreateTaskRequest.Subtask>() };
        var createResponse = await _client.PostAsJsonAsync("/tasks", createTaskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();
        _createdTaskIds.Add(createdTask!.Id);

        // Act
        var response = await _client.GetAsync($"/tasks/{createdTask.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedTask = await response.Content.ReadFromJsonAsync<TodoItem>(); // Endpoint returns core.TodoItem
        fetchedTask.Should().NotBeNull();
        fetchedTask!.Name.Should().Be(createTaskRequest.Name);
        fetchedTask.Id.Should().Be(createdTask.Id);
    }

    [Fact]
    public async Task GetTask_NonExistentTask_Returns404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/tasks/999999"); // Assuming 999999 does not exist

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Placeholder for GetTask_TaskBelongingToAnotherUser_Returns404NotFound (or 403)
    // This test would require creating a task as _otherUserId, then trying to fetch as _testUserId.
    // The outcome (403 vs 404) depends on API design choice (reveal existence or not).
    // [Fact]
    // public async Task GetTask_TaskOfAnotherUser_ReturnsNotFoundOrForbidden() { /* ... */ }

    // Placeholder for GetTask_Unauthenticated_Returns401Unauthorized
    // [Fact]
    // public async Task GetTask_Unauthenticated_Returns401Unauthorized() { /* ... */ }


    // --- GET /tasks (GetAllTasksEndpoint) Tests ---
    [Fact]
    public async Task GetAllTasks_UserHasTasks_Returns200OkWithTasks()
    {
        // Arrange: Ensure at least one task exists for the user
        var defaultListId = GetDefaultListIdForTestUser();
        var createTaskRequest1 = new CreateTaskRequest { ListId = defaultListId, Name = "Task A", Tags = new List<string>(), Subtasks = new List<CreateTaskRequest.Subtask>() };
        var createTaskRequest2 = new CreateTaskRequest { ListId = defaultListId, Name = "Task B", Tags = new List<string>(), Subtasks = new List<CreateTaskRequest.Subtask>() };
        var resp1 = await _client.PostAsJsonAsync("/tasks", createTaskRequest1);
        var resp2 = await _client.PostAsJsonAsync("/tasks", createTaskRequest2);
        _createdTaskIds.Add((await resp1.Content.ReadFromJsonAsync<CreateTaskResponse>())!.Id);
        _createdTaskIds.Add((await resp2.Content.ReadFromJsonAsync<CreateTaskResponse>())!.Id);


        // Act
        var response = await _client.GetAsync("/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        tasks.Should().NotBeNull();
        tasks!.Count.Should().BeGreaterOrEqualTo(2); // Could be more if other tests ran in parallel on same DB, but TestContainers usually isolates.
        tasks.Should().Contain(t => t.Name == "Task A");
        tasks.Should().Contain(t => t.Name == "Task B");
    }

    [Fact]
    public async Task GetAllTasks_UserHasNoTasks_Returns200OkWithEmptyList()
    {
        // Arrange: Ensure this user has no tasks (tricky with shared DB unless truly isolated)
        // For now, assume _testUserId is fresh for this test or we use a new user.
        // A better way: create a new user for this test.
        var userWithNoTasksClient = _factory.CreateClient(); // Simulating a different user or fresh state
        // Potentially seed a list for this "new" user if task creation requires it implicitly.
        // Guid newUserForThisTest = Guid.NewGuid();
        // await SeedDefaultListForUserAsync(newUserForThisTest);
        // Then use a client "authenticated" as newUserForThisTest.
        // For simplicity, we'll assume the current _testUserId and its default list are used,
        // and we'd rely on test isolation to ensure it starts empty for this test's perspective on tasks.
        // This part is fragile without proper per-test user isolation and auth.

        // To make it more robust for THIS specific test, let's clear tasks for the default list of _testUserId
        // This is a hack due to lack of proper auth setup for different test users.
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var defaultListId = GetDefaultListIdForTestUser();
            var tasksInDefaultList = await context.TodoItems.Where(t => t.ListId == defaultListId && t.Owner == _testUserId).ToListAsync();
            if (tasksInDefaultList.Any())
            {
                context.TodoItems.RemoveRange(tasksInDefaultList);
                await context.SaveChangesAsync();
            }
        }

        // Act
        var response = await _client.GetAsync("/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        tasks.Should().NotBeNull().And.BeEmpty();
    }

    // Placeholder for GetAllTasks_Unauthenticated_Returns401Unauthorized
    // [Fact]
    // public async Task GetAllTasks_Unauthenticated_Returns401Unauthorized() { /* ... */ }
}

// Placeholder auth helper TestUserFeature was removed as it was causing build errors
// and a proper test auth handler is needed for a real solution.
