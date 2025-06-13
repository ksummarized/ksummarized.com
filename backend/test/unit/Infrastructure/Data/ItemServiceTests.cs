using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Xunit;
using core;
using core.Ports;
using infrastructure.Data;

namespace UnitTests.Infrastructure.Data
{
    public class ItemServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ItemService _itemService;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _otherUserId = Guid.NewGuid();

        public ItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            SeedDatabase(_dbContext);

            _itemService = new ItemService(_dbContext);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            var tags = new List<TagModel>
            {
                new TagModel { Id = 1, Name = "Tag1", Owner = _testUserId },
                new TagModel { Id = 2, Name = "Tag2", Owner = _testUserId },
                new TagModel { Id = 3, Name = "TagForOtherUser", Owner = _otherUserId },
            };
            context.Tags.AddRange(tags);
            context.SaveChanges();

            var items = new List<TodoItemModel>
            {
                new TodoItemModel { Id = 1, Name = "Test Item 1", Owner = _testUserId, ListId = 1, Notes = "", Tags = new List<TagModel> { tags.Single(t=>t.Name=="Tag1") }, Subtasks = new List<TodoItemModel>() },
                new TodoItemModel { Id = 2, Name = "Test Item 2", Owner = _testUserId, ListId = 1, Completed = true, Notes = "Completed item notes", Tags = new List<TagModel> { tags.Single(t=>t.Name=="Tag2") }, Subtasks = new List<TodoItemModel>() },
                new TodoItemModel { Id = 3, Name = "Test Item 3 OtherUser", Owner = _otherUserId, ListId = 2, Notes = "Other user's item", Tags = new List<TagModel> {tags.Single(t=>t.Name=="TagForOtherUser")}, Subtasks = new List<TodoItemModel>()},
                new TodoItemModel { Id = 4, Name = "Test Item 4 With Subtask", Owner = _testUserId, ListId = 1, Notes = "Item with a subtask", Tags = new List<TagModel>(), Subtasks = new List<TodoItemModel>
                    {
                        new TodoItemModel { Id = 5, Name = "Subtask 1 for Item 4", Owner = _testUserId, ListId = 1, Notes = "Notes for subtask", Tags = new List<TagModel>(), Subtasks = new List<TodoItemModel>()}
                    }
                }
            };
            context.TodoItems.AddRange(items);
            context.SaveChanges();
        }

        // --- CreateItem Tests ---
        [Fact]
        public async Task CreateItem_ShouldAddItemToDatabase_AndReturnMappedItem()
        {
            // Arrange
            var newItemCore = new TodoItem
            {
                Name = "New Item Create",
                Completed = false,
                Deadline = DateTime.UtcNow.AddDays(5),
                Notes = "Some notes for new item",
                Tags = new List<core.Tag> { new core.Tag { Name = "Tag1" } }, // Existing by name
                Subtasks = new List<TodoItem> { new TodoItem { Name = "New Subtask Create", ListId = 10, Notes = "Subtask notes", Tags = new List<core.Tag>(), Subtasks = new List<TodoItem>() } },
                ListId = 10
            };

            // Act
            var result = await _itemService.CreateItem(_testUserId, newItemCore);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(newItemCore.Name);
            result.Completed.Should().BeFalse();
            result.Notes.Should().Be(newItemCore.Notes);
            result.Tags.Should().HaveCount(1);
            result.Tags.First().Name.Should().Be("Tag1");

            var createdItemModel = await _dbContext.TodoItems.Include(i => i.Tags).SingleAsync(i => i.Id == result.Id);
            createdItemModel.Owner.Should().Be(_testUserId);
            createdItemModel.Notes.Should().Be(newItemCore.Notes);
            createdItemModel.Tags.First().Id.Should().Be(1); // Tag1 has Id 1

            result.Subtasks.Should().HaveCount(1);
            result.Subtasks.First().Name.Should().Be("New Subtask Create");
            result.Subtasks.First().Notes.Should().Be("Subtask notes");

            var dbItem = await _dbContext.TodoItems.Include(i => i.Subtasks).FirstOrDefaultAsync(i => i.Id == result.Id);
            dbItem.Should().NotBeNull();
            dbItem!.Name.Should().Be(newItemCore.Name);
            dbItem.Subtasks.Should().HaveCount(1);
            dbItem.Subtasks.First().MainTaskId.Should().Be(result.Id);
        }

        [Fact]
        public async Task CreateItem_WithNewTag_ShouldCreateTagAndAddItem()
        {
            // Arrange
            var newItemCore = new TodoItem
            {
                Name = "Item With NewTag Create",
                Notes = "Notes for item with new tag",
                Tags = new List<core.Tag> { new core.Tag { Name = "CompletelyNewTag" } },
                Subtasks = new List<TodoItem>(),
                ListId = 1
            };

            // Act
            var result = await _itemService.CreateItem(_testUserId, newItemCore);

            // Assert
            result.Should().NotBeNull();
            result.Tags.Should().ContainSingle(t => t.Name == "CompletelyNewTag");

            var dbItem = await _dbContext.TodoItems.Include(i => i.Tags).SingleAsync(i => i.Id == result.Id);
            dbItem.Notes.Should().Be("Notes for item with new tag");
            dbItem.Tags.Should().ContainSingle(t => t.Name == "CompletelyNewTag" && t.Owner == _testUserId && t.Id != 0);
            var newTag = dbItem.Tags.First(t => t.Name == "CompletelyNewTag");
            newTag.Id.Should().BeGreaterThan(3);
        }

        // --- GetItem Tests ---
        [Fact]
        public async Task GetItem_ExistingItemForUser_ShouldReturnMappedItem()
        {
            // Arrange
            var itemId = 1;

            // Act
            var result = await _itemService.GetItem(_testUserId, itemId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(itemId);
            result.Name.Should().Be("Test Item 1");
            result.Notes.Should().Be("");
            result.Tags.Should().HaveCount(1);
            result.Tags.First().Name.Should().Be("Tag1");
        }

        [Fact]
        public async Task GetItem_ItemWithSubtasks_ShouldReturnMappedItemWithSubtasks()
        {
            // Arrange
            var itemId = 4;

            // Act
            var result = await _itemService.GetItem(_testUserId, itemId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(itemId);
            result.Name.Should().Be("Test Item 4 With Subtask");
            result.Notes.Should().Be("Item with a subtask");
            result.Subtasks.Should().HaveCount(1);
            result.Subtasks.First().Name.Should().Be("Subtask 1 for Item 4");
            result.Subtasks.First().Id.Should().Be(5);
            result.Subtasks.First().Notes.Should().Be("Notes for subtask");
        }


        [Fact]
        public async Task GetItem_NonExistingItem_ShouldReturnNull()
        {
            // Arrange
            var itemId = 999;

            // Act
            var result = await _itemService.GetItem(_testUserId, itemId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetItem_ItemBelongingToAnotherUser_ShouldReturnNull()
        {
            // Arrange
            var itemId = 3;

            // Act
            var result = await _itemService.GetItem(_testUserId, itemId);

            // Assert
            result.Should().BeNull();
        }

        // --- ListItems Tests ---
        [Fact]
        public void ListItems_NoFilters_ShouldReturnAllUserItemsNotSubtasks()
        {
            // Act
            var result = _itemService.ListItems(_testUserId, null, null).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(i => i.Id).Should().BeEquivalentTo(new[] { 1, 2, 4 });
        }

        [Fact]
        public void ListItems_FilterByTag_ShouldReturnMatchingItems()
        {
            // Arrange
            var tagIdToFilterBy = 1;

            // Act
            var result = _itemService.ListItems(_testUserId, tagIdToFilterBy, null).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
            result.First().Tags.Should().ContainSingle(t => t.Id == tagIdToFilterBy);
        }

        [Fact]
        public void ListItems_FilterByCompleted_ShouldReturnMatchingItems()
        {
            // Act
            var result = _itemService.ListItems(_testUserId, null, true).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(2);
            result.First().Completed.Should().BeTrue();
        }

        [Fact]
        public void ListItems_FilterByNotCompleted_ShouldReturnMatchingItems()
        {
            // Act
            var result = _itemService.ListItems(_testUserId, null, false).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(i => !i.Completed);
            result.Select(i => i.Id).Should().BeEquivalentTo(new[] { 1, 4 });
        }

        // --- UpdateItem Tests ---
        [Fact]
        public async Task UpdateItem_ExistingItem_ShouldUpdateAndReturnTrue()
        {
            // Arrange
            var itemToUpdate = new TodoItem
            {
                Id = 1, // Test Item 1, initially has Tag1 (Id=1)
                Name = "Updated Item Name",
                Completed = true,
                Deadline = DateTime.UtcNow.AddDays(10),
                Notes = "Updated notes",
                // DTO specifies adding Tag2 (Id=2). Tag1 is not mentioned in DTO.
                Tags = new List<core.Tag> { new core.Tag { Id = 2, Name = "Tag2" } },
                Subtasks = new List<TodoItem>(),
                ListId = 101
            };

            // Act
            var success = await _itemService.UpdateItem(_testUserId, itemToUpdate);

            // Assert
            success.Should().BeTrue();
            var updatedItemDb = await _dbContext.TodoItems.Include(i=>i.Tags).SingleAsync(i => i.Id == 1);
            updatedItemDb.Name.Should().Be("Updated Item Name");
            updatedItemDb.Completed.Should().BeTrue();
            updatedItemDb.Notes.Should().Be("Updated notes");
            updatedItemDb.ListId.Should().Be(101);

            // Verify current service behavior: adds new tags, does not remove existing ones not in DTO.
            // Item1 started with Tag1 (Id=1). DTO added Tag2 (Id=2).
            updatedItemDb.Tags.Should().HaveCount(2);
            updatedItemDb.Tags.Should().Contain(t => t.Id == 1 && t.Name == "Tag1"); // Original tag
            updatedItemDb.Tags.Should().Contain(t => t.Id == 2 && t.Name == "Tag2"); // Added tag
        }

        [Fact]
        public async Task UpdateItem_AddingNewSubtaskToExistingItem_ShouldUpdateItemAndAddSubtask()
        {
            // Arrange
            var itemToModify = await _dbContext.TodoItems.Include(i => i.Subtasks).Include(i => i.Tags).SingleAsync(i => i.Id == 1);
            itemToModify.Subtasks.Clear(); // Ensure Item1 starts with no subtasks for this test
            await _dbContext.SaveChangesAsync();

             var itemToUpdate = new TodoItem
            {
                Id = 1,
                Name = "Item 1 with new subtask",
                Notes = itemToModify.Notes,
                ListId = itemToModify.ListId,
                Tags = itemToModify.Tags.Select(t => new core.Tag { Id = t.Id, Name = t.Name }).ToList(),
                Subtasks = new List<TodoItem> { new TodoItem { Name = "Newly Added Subtask by Update", Notes = "subtask notes upd", ListId = itemToModify.ListId, Tags = new List<core.Tag>(), Subtasks = new List<TodoItem>() } }
            };

            // Act
            var success = await _itemService.UpdateItem(_testUserId, itemToUpdate);

            // Assert
            success.Should().BeTrue();
            var updatedItem = await _itemService.GetItem(_testUserId, 1);
            updatedItem.Should().NotBeNull();
            updatedItem!.Name.Should().Be("Item 1 with new subtask");
            updatedItem.Subtasks.Should().HaveCount(1);
            updatedItem.Subtasks.First().Name.Should().Be("Newly Added Subtask by Update");
            updatedItem.Subtasks.First().Notes.Should().Be("subtask notes upd");
            updatedItem.Subtasks.First().Id.Should().NotBe(0);
        }

        [Fact]
        public async Task UpdateItem_NonExistingItem_ShouldReturnFalse()
        {
            // Arrange
            var itemToUpdate = new TodoItem { Id = 999, Name = "Non Existent Update", Notes = "notes", Tags = new List<core.Tag>(), Subtasks = new List<TodoItem>() };

            // Act
            var success = await _itemService.UpdateItem(_testUserId, itemToUpdate);

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateItem_AttemptToUpdateAnotherUserItem_ShouldReturnFalse()
        {
            // Arrange
            var itemToUpdate = new TodoItem { Id = 3, Name = "Attempt Update Other User Item", Notes = "notes", Tags = new List<core.Tag>(), Subtasks = new List<TodoItem>() };

            // Act
            var success = await _itemService.UpdateItem(_testUserId, itemToUpdate);

            // Assert
            success.Should().BeFalse();
            var originalItem = await _dbContext.TodoItems.AsNoTracking().SingleAsync(i => i.Id == 3);
            originalItem.Name.Should().Be("Test Item 3 OtherUser");
        }

        // --- DeleteItem Tests ---
        [Fact]
        public async Task DeleteItem_ExistingItemForUser_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var itemIdToDelete = 2;

            // Act
            var success = await _itemService.DeleteItem(_testUserId, itemIdToDelete);

            // Assert
            success.Should().BeTrue();
            var deletedItem = await _dbContext.TodoItems.FindAsync(itemIdToDelete);
            deletedItem.Should().BeNull();
        }

        [Fact]
        public async Task DeleteItem_ExistingItemWithSubtasks_ShouldDeleteSubtasksAndReturnTrue()
        {
            // Arrange
            var itemIdToDelete = 4;

            // Act
            var success = await _itemService.DeleteItem(_testUserId, itemIdToDelete);

            // Assert
            success.Should().BeTrue();
            var deletedItem = await _dbContext.TodoItems.FindAsync(itemIdToDelete);
            deletedItem.Should().BeNull();

            var deletedSubtask = await _dbContext.TodoItems.FindAsync(5);
            deletedSubtask.Should().BeNull("Subtask should also be deleted");
        }

        [Fact]
        public async Task DeleteItem_NonExistingItem_ShouldReturnFalse()
        {
            // Arrange
            var itemIdToDelete = 999;

            // Act
            var success = await _itemService.DeleteItem(_testUserId, itemIdToDelete);

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteItem_AttemptToDeleteAnotherUserItem_ShouldReturnFalse()
        {
            // Arrange
            var itemIdToDelete = 3;

            // Act
            var success = await _itemService.DeleteItem(_testUserId, itemIdToDelete);

            // Assert
            success.Should().BeFalse();
            var itemStillExists = await _dbContext.TodoItems.FindAsync(itemIdToDelete);
            itemStillExists.Should().NotBeNull();
        }
    }
}
