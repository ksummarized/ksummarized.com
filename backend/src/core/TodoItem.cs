namespace core;

public record TodoItem
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public bool Completed { get; set; }
    public DateTime Deadline { get; set; }
    public string Notes { get; set; } = null!;
    public required IEnumerable<Tag> Tags { get; set; }
    public required IEnumerable<TodoItem> Subtasks { get; set; }
    public int ListId { get; set; }
}
