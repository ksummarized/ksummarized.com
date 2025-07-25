using core;

namespace unit;

public class TodoListTests
{
    [Fact]
    public void Can_Create_TodoList()
    {
        var ownerId = Guid.NewGuid();
        var list = new TodoList
        {
            Name = "Test List",
            Owner = ownerId,
            Items = new List<TodoItem>()
        };

        Assert.Equal("Test List", list.Name);
        Assert.Equal(ownerId, list.Owner);
        Assert.NotNull(list.Items);
    }
}
