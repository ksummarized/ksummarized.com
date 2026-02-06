using System.Net;
using System.Net.Http.Json;
using contracts.Requests;
using contracts.Responses;
using Xunit;

namespace Ksummarized.IntegrationTests;

[Collection(IntegrationTestCollection.CollectionName)]
public class ListsEndpointsTests : IntegrationTestBase
{
    public ListsEndpointsTests(PostgresContainerFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateList_And_GetAll_ReturnsNewList()
    {
        var createResponse = await Client.PostAsJsonAsync("/api/todo/lists", new CreateListRequest("Inbox"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<GetListResponse>();
        Assert.NotNull(created);
        Assert.Equal("Inbox", created!.Name);

        var listsResponse = await Client.GetFromJsonAsync<List<GetListResponse>>("/api/todo/lists");
        Assert.NotNull(listsResponse);
        Assert.Contains(listsResponse!, list => list.Id == created.Id && list.Name == "Inbox");
    }

    [Fact]
    public async Task GetList_ReturnsItems()
    {
        var list = await CreateListAsync("Work");
        await CreateTaskAsync(list.Id, "Task One");
        await CreateTaskAsync(list.Id, "Task Two");

        var getResponse = await Client.GetFromJsonAsync<GetListResponse>($"/api/todo/lists/{list.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal(list.Id, getResponse!.Id);
        Assert.Equal(2, getResponse.Items.Count());
    }

    [Fact]
    public async Task RenameList_UpdatesName()
    {
        var list = await CreateListAsync("Home");

        var renameResponse = await Client.PutAsJsonAsync(
            $"/api/todo/lists/{list.Id}",
            new ListRenameRequest { Name = "Personal" }
        );
        Assert.Equal(HttpStatusCode.OK, renameResponse.StatusCode);

        var getResponse = await Client.GetFromJsonAsync<GetListResponse>($"/api/todo/lists/{list.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal("Personal", getResponse!.Name);
    }

    [Fact]
    public async Task DeleteList_RemovesList()
    {
        var list = await CreateListAsync("Temp");

        var deleteResponse = await Client.DeleteAsync($"/api/todo/lists/{list.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await Client.GetAsync($"/api/todo/lists/{list.Id}");
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

    private async Task CreateTaskAsync(int listId, string name)
    {
        var response = await Client.PostAsJsonAsync("/api/todo/items", new CreateTaskRequest
        {
            ListId = listId,
            Name = name,
            Deadline = null,
            Notes = string.Empty,
            Tags = Array.Empty<string>(),
            Subtasks = Array.Empty<CreateTaskRequest.Subtask>()
        });
        response.EnsureSuccessStatusCode();
    }
}
