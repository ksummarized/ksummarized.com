using System.ComponentModel.DataAnnotations;

namespace infrastructure.Data;

public class Tag {
    [Key]
    public int Id { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
    public required IEnumerable<TodoItemModel> Items {get; set;}
}