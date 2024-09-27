namespace core;

public class TodoList
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
    public required IEnumerable<TodoItem> Items { get; set; }
}
