using System.Net;
using System.Net.Http.Json;
using contracts.Requests;
using core;
using Xunit;

namespace Ksummarized.IntegrationTests;

[Collection(IntegrationTestCollection.CollectionName)]
public class TagsEndpointsTests : IntegrationTestBase
{
    public TagsEndpointsTests(PostgresContainerFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateTag_And_GetAll_ReturnsTag()
    {
        var createResponse = await Client.PostAsJsonAsync("/api/todo/tags", new CreateTagRequest("urgent"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<Tag>();
        Assert.NotNull(created);

        var tags = await Client.GetFromJsonAsync<List<Tag>>("/api/todo/tags");
        Assert.NotNull(tags);
        Assert.Contains(tags!, tag => tag.Id == created!.Id && tag.Name == "urgent");
    }

    [Fact]
    public async Task UpdateAndDeleteTag_Works()
    {
        var tag = await CreateTagAsync("old");

        var updateResponse = await Client.PutAsJsonAsync($"/api/todo/tags/{tag.Id}", new UpdateTagRequest("new"));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await Client.GetFromJsonAsync<Tag>($"/api/todo/tags/{tag.Id}");
        Assert.NotNull(updated);
        Assert.Equal("new", updated!.Name);

        var deleteResponse = await Client.DeleteAsync($"/api/todo/tags/{tag.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await Client.GetAsync($"/api/todo/tags/{tag.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }

    private async Task<Tag> CreateTagAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/todo/tags", new CreateTagRequest(name));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Tag>();
        Assert.NotNull(created);
        return created!;
    }
}
