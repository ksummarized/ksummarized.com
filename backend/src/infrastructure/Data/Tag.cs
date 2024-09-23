using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.Data;

[Table("Tags")]
public class Tag
{
    [Key]
    public int Id { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
    public ICollection<TodoItemModel> Items { get; set; } = null!;
}
