using core;

namespace api.Responses;

public record TodoList(int Id, string Name, IEnumerable<TodoItem> Items);

public static class MapExtensions
{
    public static TodoList ToResponse(this core.TodoList list) => new TodoList(list.Id, list.Name, list.Items);
}
