using api.Data.DAO.Identity;
using System.ComponentModel.DataAnnotations;

namespace api.Data.DAO.ToDo;

public class ToDoList
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(1024)]
    public required string Name { get; set; }
    public required string UserId { get; set; }
    public UserModel? User { get; set; }
}
