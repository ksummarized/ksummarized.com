namespace api.Resonses;

public record TodoList(int Id, string Name);

public static class MapExtensions
{
    public static TodoList ToResponse(this core.TodoList list) => new TodoList(list.Id, list.Name);
}
