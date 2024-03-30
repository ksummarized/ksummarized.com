using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.DAO;

[Table("todo_lists")]
public class TodoListModel
{
    [Key]
    public int Id { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
}
