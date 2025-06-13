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
using core; // For Tag DTO
using Microsoft.AspNetCore.Mvc.Testing;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Assuming TProgram is Program from api.csproj
public class TagEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;
    private Guid _testUserId;

    private readonly List<int> _createdTagIds = new();

    public TagEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync()
    {
        _testUserId = Guid.NewGuid();
        _client = _factory.CreateClient();
        // Authentication conceptual placeholder:
        // Assume _client is authenticated for _testUserId via CustomWebApplicationFactory setup (not implemented here)
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Clean up seeded tags if necessary, though Testcontainers should isolate.
        // If any tags were created directly through DbContext for setup, clear them.
        if (_createdTagIds.Any())
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var tagsToRemove = await context.Tags.Where(t => _createdTagIds.Contains(t.Id)).ToListAsync();
            if (tagsToRemove.Any())
            {
                context.Tags.RemoveRange(tagsToRemove);
                await context.SaveChangesAsync();
            }
        }
    }

    private async Task<TagModel> SeedTagAsync(string tagName, Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var tag = new TagModel { Name = tagName, Owner = userId };
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        _createdTagIds.Add(tag.Id); // Track for potential cleanup
        return tag;
    }

    // --- POST /tags (CreateTagEndpoint) Tests ---
    [Fact]
    public async Task CreateTag_ValidName_Returns201CreatedWithTag()
    {
        // Arrange
        var request = new CreateTagRequest("NewLabel");

        // Act
        var response = await _client.PostAsJsonAsync("/tags", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTag = await response.Content.ReadFromJsonAsync<Tag>();

        createdTag.Should().NotBeNull();
        createdTag!.Name.Should().Be(request.Name);
        createdTag.Id.Should().NotBe(0);
        _createdTagIds.Add(createdTag.Id); // Track for cleanup

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be($"/tags/{createdTag.Id}");

        // Verify in DB
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbTag = await context.Tags.FindAsync(createdTag.Id);
        dbTag.Should().NotBeNull();
        dbTag!.Owner.Should().Be(_testUserId); // Assuming service sets owner from HttpContext
    }

    [Fact]
    public async Task CreateTag_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var request = new CreateTagRequest(""); // Empty name

        // Act
        var response = await _client.PostAsJsonAsync("/tags", request);

        // Assert
        // Depends on API validation (e.g., [Required] or FluentValidation on DTO)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Test for unauthenticated: POST /tags
    // [Fact]
    // public async Task CreateTag_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // --- GET /tags/{id} (GetTagEndpoint) Tests ---
    [Fact]
    public async Task GetTag_ExistingTag_Returns200OkWithTag()
    {
        // Arrange
        var seededTag = await SeedTagAsync("ExistingTag", _testUserId);

        // Act
        var response = await _client.GetAsync($"/tags/{seededTag.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tag = await response.Content.ReadFromJsonAsync<Tag>();
        tag.Should().NotBeNull();
        tag!.Id.Should().Be(seededTag.Id);
        tag.Name.Should().Be(seededTag.Name);
    }

    [Fact]
    public async Task GetTag_NonExistentTag_Returns404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/tags/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test for unauthorized access: GET /tags/{id}
    // [Fact]
    // public async Task GetTag_TagBelongingToAnotherUser_Returns404NotFound() { /* ... */ }

    // Test for unauthenticated: GET /tags/{id}
    // [Fact]
    // public async Task GetTag_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // --- GET /tags (GetAllTagsEndpoint) Tests ---
    [Fact]
    public async Task GetAllTags_UserHasTags_Returns200OkWithTags()
    {
        // Arrange
        await SeedTagAsync("TagAlpha", _testUserId);
        await SeedTagAsync("TagBeta", _testUserId);

        // Act
        var response = await _client.GetAsync("/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tags = await response.Content.ReadFromJsonAsync<List<Tag>>();
        tags.Should().NotBeNull();
        tags!.Should().Contain(t => t.Name == "TagAlpha");
        tags!.Should().Contain(t => t.Name == "TagBeta");
    }

    [Fact]
    public async Task GetAllTags_UserHasNoTags_Returns200OkWithEmptyList()
    {
        // Arrange (ensure user has no tags - relies on test isolation or specific user for this test)
        // For this test, we assume _testUserId is fresh or its tags are cleared by DisposeAsync if it ran prior tests.
        // Alternatively, create a new user ID for this specific test.
        // For now, we'll rely on the current _testUserId and whatever state it's in from previous tests or InitializeAsync.
        // This might need adjustment if tests interfere. A truly clean test would use a unique user.
        // Let's clear tags for the current _testUserId to make this test more robust.
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userTags = await context.Tags.Where(t => t.Owner == _testUserId).ToListAsync();
            if(userTags.Any()) context.Tags.RemoveRange(userTags);
            await context.SaveChangesAsync();
        }
        _createdTagIds.Clear();


        // Act
        var response = await _client.GetAsync("/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tags = await response.Content.ReadFromJsonAsync<List<Tag>>();
        tags.Should().NotBeNull().And.BeEmpty();
    }

    // Test for unauthenticated: GET /tags
    // [Fact]
    // public async Task GetAllTags_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // --- PUT /tags/{id} (UpdateTagEndpoint) Tests ---
    [Fact]
    public async Task UpdateTag_ExistingTag_ValidName_Returns200Ok()
    {
        // Arrange
        var seededTag = await SeedTagAsync("OldTagName", _testUserId);
        var request = new UpdateTagRequest("NewTagName");

        // Act
        var response = await _client.PutAsJsonAsync($"/tags/{seededTag.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbTag = await context.Tags.FindAsync(seededTag.Id);
        dbTag!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateTag_NonExistentTag_Returns404NotFound()
    {
        // Arrange
        var request = new UpdateTagRequest("UpdateForNonExistent");

        // Act
        var response = await _client.PutAsJsonAsync("/tags/999999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTag_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var seededTag = await SeedTagAsync("TagToUpdateWithEmpty", _testUserId);
        var request = new UpdateTagRequest("");

        // Act
        var response = await _client.PutAsJsonAsync($"/tags/{seededTag.Id}", request);

        // Assert
        // Depends on API validation for UpdateTagRequest.Name
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Test for unauthenticated: PUT /tags/{id}
    // [Fact]
    // public async Task UpdateTag_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // Test for unauthorized: PUT /tags/{id} - tag belongs to another user
    // [Fact]
    // public async Task UpdateTag_TagBelongingToAnotherUser_Returns404NotFound() { /* ... */ }


    // --- DELETE /tags/{id} (DeleteTagEndpoint) Tests ---
    [Fact]
    public async Task DeleteTag_ExistingTag_Returns204NoContent()
    {
        // Arrange
        var seededTag = await SeedTagAsync("TagToDelete", _testUserId);
         _createdTagIds.Remove(seededTag.Id); // Will be deleted by API, no need for DisposeAsync to try again.


        // Act
        var response = await _client.DeleteAsync($"/tags/{seededTag.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbTag = await context.Tags.FindAsync(seededTag.Id);
        dbTag.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTag_NonExistentTag_Returns404NotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/tags/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test for unauthenticated: DELETE /tags/{id}
    // [Fact]
    // public async Task DeleteTag_Unauthenticated_Returns401Unauthorized() { /* ... */ }

    // Test for unauthorized: DELETE /tags/{id} - tag belongs to another user
    // [Fact]
    // public async Task DeleteTag_TagBelongingToAnotherUser_Returns404NotFound() { /* ... */ }
}
