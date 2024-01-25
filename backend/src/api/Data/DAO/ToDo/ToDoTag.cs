using System.ComponentModel.DataAnnotations;

namespace api.Data.DAO.ToDo;

public class ToDoTag
{
    [Key]
    public int Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }

}
