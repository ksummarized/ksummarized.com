using core;

namespace unit;

public class TodoItemTests
{
    [Fact]
    public void Can_Create_TodoItem()
    {
        var item = new TodoItem
        {
            Name = "Test Todo",
            Tags = new List<Tag>(),
            Subtasks = new List<TodoItem>(),
            ListId = 1
        };

        Assert.Equal("Test Todo", item.Name);
        Assert.False(item.Completed);
        Assert.Null(item.Deadline);
        Assert.NotNull(item.Tags);
        Assert.NotNull(item.Subtasks);
        Assert.Equal(1, item.ListId);
    }
}
