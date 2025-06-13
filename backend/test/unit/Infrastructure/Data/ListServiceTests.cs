using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using core;
using core.Ports;
using infrastructure.Data;

namespace UnitTests.Infrastructure.Data
{
    public class ListServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ListService _listService;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _otherUserId = Guid.NewGuid();

        public ListServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            SeedDatabase(_dbContext);

            _listService = new ListService(_dbContext);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            var tags = new List<TagModel>
            {
                new TagModel { Id = 1, Name = "Tag1", Owner = _testUserId },
                new TagModel { Id = 2, Name = "Tag2", Owner = _testUserId },
            };
            context.Tags.AddRange(tags);
            context.SaveChanges();

            var lists = new List<TodoListModel>
            {
                new TodoListModel { Id = 1, Name = "Test List 1", Owner = _testUserId, Items = new List<TodoItemModel>
                    {
                        new TodoItemModel { Id = 1, Name = "Item 1.1", Owner = _testUserId, ListId = 1, Notes = "Note 1.1", Tags = new List<TagModel> { tags[0] }, Subtasks = new List<TodoItemModel>() },
                        new TodoItemModel { Id = 2, Name = "Item 1.2", Owner = _testUserId, ListId = 1, Notes = "Note 1.2", Completed = true, Tags = new List<TagModel> { tags[1] }, Subtasks = new List<TodoItemModel>() }
                    }
                },
                new TodoListModel { Id = 2, Name = "Test List 2", Owner = _testUserId, Items = new List<TodoItemModel>
                    {
                        new TodoItemModel { Id = 3, Name = "Item 2.1", Owner = _testUserId, ListId = 2, Notes = "Note 2.1", Tags = new List<TagModel>(), Subtasks = new List<TodoItemModel>() }
                    }
                },
                new TodoListModel { Id = 3, Name = "Other User List", Owner = _otherUserId, Items = new List<TodoItemModel>() }
            };
            context.TodoLists.AddRange(lists);
            context.SaveChanges();
        }

        // --- CreateList Tests ---
        [Fact]
        public async Task CreateList_ValidInput_ShouldCreateAndReturnList()
        {
            // Arrange
            var listName = "New Shopping List";

            // Act
            var result = await _listService.CreateList(_testUserId, listName);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(listName);
            result.Owner.Should().Be(_testUserId);
            result.Id.Should().NotBe(0);
            result.Items.Should().BeEmpty();

            var dbList = await _dbContext.TodoLists.FindAsync(result.Id);
            dbList.Should().NotBeNull();
            dbList!.Name.Should().Be(listName);
            dbList.Owner.Should().Be(_testUserId);
        }

        // --- GetLists Tests ---
        [Fact]
        public void GetLists_UserWithLists_ShouldReturnUserListsWithoutItems()
        {
            // Act
            var result = _listService.GetLists(_testUserId).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.Owner == _testUserId);
            result.ForEach(l => l.Items.Should().BeEmpty());
        }

        [Fact]
        public void GetLists_UserWithoutLists_ShouldReturnEmpty()
        {
            // Arrange
            var userWithNoLists = Guid.NewGuid();

            // Act
            var result = _listService.GetLists(userWithNoLists).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // --- GetList (with GetListOptions) Tests ---
        [Fact]
        public void GetList_ExistingListForUser_DefaultOptions_ShouldReturnListWithItems()
        {
            // Arrange
            var options = new GetListOptions(
                UserId: _testUserId,
                ListId: 1,
                Tag: null,
                Completed: null,
                Page: 1,
                PageSize: 10,
                IncludeSubtasks: false
            );

            // Act
            var result = _listService.GetList(options);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test List 1");
            result.Owner.Should().Be(_testUserId);
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(i => i.Name == "Item 1.1");
        }

        [Fact]
        public void GetList_FilterByTag_ShouldReturnFilteredItems()
        {
            // Arrange
            var options = new GetListOptions(
                UserId: _testUserId,
                ListId: 1,
                Tag: 1, // Tag1
                Completed: null,
                Page: 1,
                PageSize: 10,
                IncludeSubtasks: false
            );

            // Act
            var result = _listService.GetList(options);

            // Assert
            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(1);
            result.Items.First().Name.Should().Be("Item 1.1");
            result.Items.First().Tags.Should().Contain(t => t.Id == 1);
        }

        [Fact]
        public void GetList_FilterByCompleted_ShouldReturnFilteredItems()
        {
            // Arrange
             var options = new GetListOptions(
                UserId: _testUserId,
                ListId: 1,
                Tag: null,
                Completed: true,
                Page: 1,
                PageSize: 10,
                IncludeSubtasks: false
            );

            // Act
            var result = _listService.GetList(options);

            // Assert
            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(1);
            result.Items.First().Name.Should().Be("Item 1.2");
            result.Items.First().Completed.Should().BeTrue();
        }

        [Fact]
        public void GetList_NonExistingList_ShouldReturnNull()
        {
            // Arrange
            var options = new GetListOptions(
                UserId: _testUserId,
                ListId: 999,
                Tag: null,
                Completed: null,
                Page: 1,
                PageSize: 10,
                IncludeSubtasks: false
            );

            // Act
            var result = _listService.GetList(options);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetList_ListBelongingToAnotherUser_ShouldReturnNull()
        {
            // Arrange
            var options = new GetListOptions(
                UserId: _testUserId, // Current user
                ListId: 3,           // List 3 belongs to _otherUserId
                Tag: null,
                Completed: null,
                Page: 1,
                PageSize: 10,
                IncludeSubtasks: false
            );

            // Act
            var result = _listService.GetList(options);

            // Assert
            result.Should().BeNull();
        }

        // --- RenameList Tests ---
        [Fact]
        public async Task RenameList_ExistingListForUser_ShouldRenameAndReturnTrue()
        {
            // Arrange
            var listIdToRename = 1;
            var newName = "Renamed List 1";

            // Act
            var success = await _listService.RenameList(_testUserId, listIdToRename, newName);

            // Assert
            success.Should().BeTrue();
            var renamedList = await _dbContext.TodoLists.FindAsync(listIdToRename);
            renamedList.Should().NotBeNull();
            renamedList!.Name.Should().Be(newName);
        }

        [Fact]
        public async Task RenameList_NonExistingList_ShouldReturnFalse()
        {
            // Arrange
            var newName = "Non Existent List";

            // Act
            var success = await _listService.RenameList(_testUserId, 999, newName);

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task RenameList_ListBelongingToAnotherUser_ShouldReturnFalse()
        {
            // Arrange
            var listIdToRename = 3;
            var newName = "Attempted Rename";

            // Act
            var success = await _listService.RenameList(_testUserId, listIdToRename, newName);

            // Assert
            success.Should().BeFalse();
            var originalList = await _dbContext.TodoLists.FindAsync(listIdToRename);
            originalList!.Name.Should().NotBe(newName);
        }

        // --- DeleteList Tests ---
        [Fact]
        public async Task DeleteList_ExistingListForUser_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var listIdToDelete = 2;

            // Act
            var success = _listService.DeleteList(_testUserId, listIdToDelete);

            // Assert
            success.Should().BeTrue();
            var deletedList = await _dbContext.TodoLists.FindAsync(listIdToDelete);
            deletedList.Should().BeNull();

            var itemsFromDeletedList = await _dbContext.TodoItems.Where(i => i.ListId == listIdToDelete).ToListAsync();
            itemsFromDeletedList.Should().BeEmpty("Items from deleted list should also be deleted if cascading is configured or handled by service/DB");
        }

        [Fact]
        public async Task DeleteList_NonExistingList_ShouldReturnFalse()
        {
            // Act
            var success = _listService.DeleteList(_testUserId, 999);

            // Assert
            success.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteList_ListBelongingToAnotherUser_ShouldReturnFalse()
        {
            // Arrange
            var listIdToDelete = 3;

            // Act
            var success = _listService.DeleteList(_testUserId, listIdToDelete);

            // Assert
            success.Should().BeFalse();
            var listStillExists = await _dbContext.TodoLists.FindAsync(listIdToDelete);
            listStillExists.Should().NotBeNull();
        }
    }
}
