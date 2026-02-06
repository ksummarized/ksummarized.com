using System.Net;
using System.Net.Http.Json;
using contracts.Requests;
using contracts.Responses;
using core;
using Xunit;

namespace Ksummarized.IntegrationTests;

[Collection(IntegrationTestCollection.CollectionName)]
public class TasksEndpointsTests : IntegrationTestBase
{
    public TasksEndpointsTests(PostgresContainerFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateTask_And_GetTask_ReturnsExpected()
    {
        var list = await CreateListAsync("Inbox");
        var request = new CreateTaskRequest
        {
            ListId = list.Id,
            Name = "Write integration tests",
            Deadline = DateTime.UtcNow.AddDays(1),
            Notes = "Focus on API coverage",
            Tags = new[] { "backend", "testing" },
            Subtasks = new[]
            {
                new CreateTaskRequest.Subtask
                {
                    Name = "Prepare fixtures",
                    Deadline = null,
                    Notes = string.Empty,
                    Tags = new[] { "backend" }
                }
            }
        };

        var createResponse = await Client.PostAsJsonAsync("/api/todo/items", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();
        Assert.NotNull(created);
        Assert.Equal(request.Name, created!.Name);
        Assert.Equal(2, created.Tags.Count());
        Assert.Single(created.Subtasks);

        var getResponse = await Client.GetFromJsonAsync<TodoItem>($"/api/todo/items/{created.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal(created.Id, getResponse!.Id);
        Assert.Equal(request.Name, getResponse.Name);
    }

    [Fact]
    public async Task UpdateTask_ChangesFields()
    {
        var list = await CreateListAsync("Inbox");
        var created = await CreateTaskAsync(list.Id, "Initial");

        var updateRequest = new TodoItem
        {
            Id = created.Id,
            Name = "Updated",
            Completed = true,
            Deadline = DateTime.UtcNow.AddDays(2),
            Notes = "Updated notes",
            Tags = created.Tags.Select(t => new Tag { Id = t.Id, Name = t.Name }).ToList(),
            Subtasks = new[]
            {
                new TodoItem
                {
                    Id = null,
                    Name = "Subtask",
                    Completed = false,
                    Deadline = null,
                    Notes = string.Empty,
                    Tags = Array.Empty<Tag>(),
                    Subtasks = Array.Empty<TodoItem>(),
                    ListId = list.Id
                }
            },
            ListId = list.Id
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/todo/items/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await Client.GetFromJsonAsync<TodoItem>($"/api/todo/items/{created.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal("Updated", getResponse!.Name);
        Assert.True(getResponse.Completed);
        Assert.Single(getResponse.Subtasks);
    }

    [Fact]
    public async Task GetAllTasks_FiltersByTagAndCompleted()
    {
        var list = await CreateListAsync("Inbox");
        var first = await CreateTaskAsync(list.Id, "First", new[] { "alpha" });
        var second = await CreateTaskAsync(list.Id, "Second", new[] { "beta" });

        var secondItem = await Client.GetFromJsonAsync<TodoItem>($"/api/todo/items/{second.Id}");
        Assert.NotNull(secondItem);
        secondItem!.Completed = true;
        var completeResponse = await Client.PutAsJsonAsync($"/api/todo/items/{second.Id}", secondItem);
        completeResponse.EnsureSuccessStatusCode();

        var tagId = first.Tags.First().Id;

        var tagged = await Client.GetFromJsonAsync<List<TodoItem>>($"/api/todo/items?tag={tagId}");
        Assert.NotNull(tagged);
        Assert.Single(tagged!);
        Assert.Equal(first.Id, tagged[0].Id);

        var completed = await Client.GetFromJsonAsync<List<TodoItem>>("/api/todo/items?completed=true");
        Assert.NotNull(completed);
        Assert.Single(completed!);
        Assert.Equal(second.Id, completed[0].Id);
    }

    [Fact]
    public async Task DeleteTask_RemovesItem()
    {
        var list = await CreateListAsync("Inbox");
        var created = await CreateTaskAsync(list.Id, "Temp");

        var deleteResponse = await Client.DeleteAsync($"/api/todo/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await Client.GetAsync($"/api/todo/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<GetListResponse> CreateListAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/todo/lists", new CreateListRequest(name));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<GetListResponse>();
        Assert.NotNull(created);
        return created!;
    }

    private async Task<CreateTaskResponse> CreateTaskAsync(int listId, string name, IEnumerable<string>? tags = null)
    {
        var response = await Client.PostAsJsonAsync("/api/todo/items", new CreateTaskRequest
        {
            ListId = listId,
            Name = name,
            Deadline = null,
            Notes = string.Empty,
            Tags = tags ?? Array.Empty<string>(),
            Subtasks = Array.Empty<CreateTaskRequest.Subtask>()
        });
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<CreateTaskResponse>();
        Assert.NotNull(created);
        return created!;
    }
}
