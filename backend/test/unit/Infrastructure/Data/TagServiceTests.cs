using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using core; // For Tag DTO
using infrastructure.Data; // For TagModel, ApplicationDbContext
// ITagService is in core.Ports, but TagService itself is being tested.

namespace UnitTests.Infrastructure.Data
{
    public class TagServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TagService _tagService;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _otherUserId = Guid.NewGuid();

        public TagServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            SeedDatabase(_dbContext);

            _tagService = new TagService(_dbContext);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            var tags = new List<TagModel>
            {
                new TagModel { Id = 1, Name = "Work", Owner = _testUserId },
                new TagModel { Id = 2, Name = "Personal", Owner = _testUserId },
                new TagModel { Id = 3, Name = "Urgent", Owner = _otherUserId },
            };
            context.Tags.AddRange(tags);

            // Seed items with tags to test if associations are handled (though TagService itself doesn't directly manage this)
            var items = new List<TodoItemModel>
            {
                new TodoItemModel { Id = 1, Name = "Item 1", Owner = _testUserId, ListId = 1, Notes = "", Tags = new List<TagModel> { tags[0] }, Subtasks = new List<TodoItemModel>() }, // Tagged "Work"
                new TodoItemModel { Id = 2, Name = "Item 2", Owner = _testUserId, ListId = 1, Notes = "", Tags = new List<TagModel> { tags[0], tags[1] }, Subtasks = new List<TodoItemModel>() } // Tagged "Work", "Personal"
            };
            context.TodoItems.AddRange(items);
            context.SaveChanges();
        }

        // --- CreateTag Tests ---
        [Fact]
        public async Task CreateTag_ValidInput_ShouldCreateAndReturnTag()
        {
            // Arrange
            var tagName = "Groceries";

            // Act
            var result = await _tagService.CreateTag(_testUserId, tagName);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(tagName);
            result.Id.Should().NotBe(0); // DB-generated ID

            var dbTag = await _dbContext.Tags.FindAsync(result.Id);
            dbTag.Should().NotBeNull();
            dbTag!.Name.Should().Be(tagName);
            dbTag.Owner.Should().Be(_testUserId);
        }

        [Fact]
        public async Task CreateTag_DuplicateNameForSameUser_ShouldStillCreateTag()
        {
            // Arrange
            var tagName = "Work"; // Already exists for _testUserId

            // Act
            var result = await _tagService.CreateTag(_testUserId, tagName);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(tagName);
            result.Id.Should().NotBe(1); // Should be a new ID, not the existing "Work" tag's ID

            var allWorkTagsForUser = await _dbContext.Tags
                .Where(t => t.Owner == _testUserId && t.Name == "Work")
                .ToListAsync();
            allWorkTagsForUser.Should().HaveCount(2);
        }

        // --- GetTag Tests ---
        [Fact]
        public async Task GetTag_ExistingTagForUser_ShouldReturnTag()
        {
            // Arrange
            var tagId = 1; // "Work" tag for _testUserId

            // Act
            var result = await _tagService.GetTag(_testUserId, tagId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(tagId);
            result.Name.Should().Be("Work");
        }

        [Fact]
        public async Task GetTag_NonExistingTag_ShouldReturnNull()
        {
            // Act
            var result = await _tagService.GetTag(_testUserId, 999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetTag_TagBelongingToAnotherUser_ShouldReturnNull()
        {
            // Arrange
            var tagId = 3; // "Urgent" tag for _otherUserId

            // Act
            var result = await _tagService.GetTag(_testUserId, tagId);

            // Assert
            result.Should().BeNull();
        }

        // --- ListTags Tests ---
        [Fact]
        public void ListTags_UserWithTags_ShouldReturnUserTags()
        {
            // Act
            var result = _tagService.ListTags(_testUserId).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // "Work", "Personal"
            result.Should().Contain(t => t.Name == "Work");
            result.Should().Contain(t => t.Name == "Personal");
        }

        [Fact]
        public void ListTags_UserWithoutTags_ShouldReturnEmpty()
        {
            // Arrange
            var userWithNoTags = Guid.NewGuid();

            // Act
            var result = _tagService.ListTags(userWithNoTags).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // --- UpdateTag Tests ---
        [Fact]
        public async Task UpdateTag_ExistingTagForUser_ShouldUpdateAndReturnTrue()
        {
            // Arrange
            var tagIdToUpdate = 1; // "Work"
            var newName = "Work Updated";

            // Act
            var success = await _tagService.UpdateTag(_testUserId, tagIdToUpdate, newName);

            // Assert
            success.Should().BeTrue();
            var updatedTag = await _dbContext.Tags.FindAsync(tagIdToUpdate);
            updatedTag.Should().NotBeNull();
            updatedTag!.Name.Should().Be(newName);
        }

        [Fact]
        public async Task UpdateTag_NonExistingTag_ShouldReturnFalse()
        {
            // Act
            var success = await _tagService.UpdateTag(_testUserId, 999, "Non Existent");

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateTag_TagBelongingToAnotherUser_ShouldReturnFalse()
        {
            // Arrange
            var tagIdToUpdate = 3; // "Urgent" for _otherUserId
            var newName = "Attempted Update";

            // Act
            var success = await _tagService.UpdateTag(_testUserId, tagIdToUpdate, newName);

            // Assert
            success.Should().BeFalse();
            var originalTag = await _dbContext.Tags.FindAsync(tagIdToUpdate);
            originalTag!.Name.Should().NotBe(newName);
        }

        [Fact]
        public async Task UpdateTag_ToExistingNameForSameUser_ShouldAllowUpdate()
        {
            // "Work" (Id=1) and "Personal" (Id=2) exist for _testUserId.
            // Renaming "Personal" (Id=2) to "Work". This should be allowed by this service,
            // resulting in two tags named "Work" for the same user.
            // Arrange
            var tagIdToUpdate = 2; // "Personal"
            var newName = "Work";

            // Act
            var success = await _tagService.UpdateTag(_testUserId, tagIdToUpdate, newName);

            // Assert
            success.Should().BeTrue();
            var updatedTag = await _dbContext.Tags.FindAsync(tagIdToUpdate);
            updatedTag!.Name.Should().Be(newName);

            var allWorkTags = await _dbContext.Tags.Where(t => t.Owner == _testUserId && t.Name == "Work").ToListAsync();
            allWorkTags.Should().HaveCount(2);
        }

        // --- DeleteTag Tests ---
        [Fact]
        public async Task DeleteTag_ExistingTagForUser_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var tagIdToDelete = 1; // "Work"

            // Act
            var success = await _tagService.DeleteTag(_testUserId, tagIdToDelete);

            // Assert
            success.Should().BeTrue();
            var deletedTag = await _dbContext.Tags.FindAsync(tagIdToDelete);
            deletedTag.Should().BeNull();

            // Verify associations are removed from items (EF Core InMemory behavior for many-to-many)
            // This requires checking items that previously had this tag.
            var item1 = await _dbContext.TodoItems.Include(i => i.Tags).SingleAsync(i => i.Id == 1);
            var item2 = await _dbContext.TodoItems.Include(i => i.Tags).SingleAsync(i => i.Id == 2);

            item1.Tags.Should().NotContain(t => t.Id == tagIdToDelete);
            item2.Tags.Should().NotContain(t => t.Id == tagIdToDelete);
        }

        [Fact]
        public async Task DeleteTag_NonExistingTag_ShouldReturnFalse()
        {
            // Act
            var success = await _tagService.DeleteTag(_testUserId, 999);

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteTag_TagBelongingToAnotherUser_ShouldReturnFalse()
        {
            // Arrange
            var tagIdToDelete = 3; // "Urgent" for _otherUserId

            // Act
            var success = await _tagService.DeleteTag(_testUserId, tagIdToDelete);

            // Assert
            success.Should().BeFalse();
            var tagStillExists = await _dbContext.Tags.FindAsync(tagIdToDelete);
            tagStillExists.Should().NotBeNull();
        }
    }
}
