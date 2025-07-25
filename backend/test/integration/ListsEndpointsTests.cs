using System.Net;
using System.Net.Http.Json;
using core;
using contracts.Responses;

namespace integration;

public class ListsEndpointsTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public ListsEndpointsTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllLists_Returns_Empty_List_When_Db_Is_Empty()
    {
        var response = await _client.GetAsync("/lists");
        var lists = await response.Content.ReadFromJsonAsync<IEnumerable<TodoList>>();

        response.EnsureSuccessStatusCode();
        Assert.NotNull(lists);
        Assert.Empty(lists);
    }

    [Fact]
    public async Task CreateList_Creates_A_New_List()
    {
        var listName = "Test List";
        var response = await _client.PostAsJsonAsync("/lists", new { Name = listName });
        var list = await response.Content.ReadFromJsonAsync<GetListResponse>();

        response.EnsureSuccessStatusCode();
        Assert.NotNull(list);
        Assert.Equal(listName, list.Name);
    }
}
