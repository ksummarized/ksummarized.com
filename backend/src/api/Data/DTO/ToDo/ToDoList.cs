using System.ComponentModel.DataAnnotations;

namespace api.Data.DTO.ToDo;

public class ToDoList
{
    public Guid Id { get; set; }

    [MaxLength(1024)]
    public required string Name { get; set; }
}
