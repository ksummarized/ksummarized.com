using System.ComponentModel.DataAnnotations;

namespace api.Data.DAO.ToDo;

public class ToDoItem
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(1024)]
    public required string Title { get; set; }
    [MaxLength(20)]
    public required string Status { get; set; }
    [MaxLength(4096)]
    public required string Notes { get; set; }
    public DateTime Deadline { get; set; }
    public Guid ListId { get; set; }
    public ToDoList? List { get; set; }
    public Guid? ParentId { get; set; }
    public ToDoItem? Parent { get; set; }
}
