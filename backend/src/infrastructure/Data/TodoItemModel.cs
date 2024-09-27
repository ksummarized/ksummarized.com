using System.ComponentModel.DataAnnotations;

namespace infrastructure.Data;

public class TodoItemModel
{
    [Key]
    public int Id { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
    public bool Compleated { get; set; }
    public DateTime Deadline { get; set; }
    [MaxLength(4096)]
    public string Notes { get; set; } = null!;
    public required ICollection<TagModel> Tags { get; set; }
    public required ICollection<TodoItemModel> Subtasks { get; set; }
    public int? MainTaskId { get; set; }
    public TodoItemModel? MainTask { get; set; }
    public int ListId { get; set; }
    public TodoListModel? List { get; set; }
}
