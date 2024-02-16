using System.ComponentModel.DataAnnotations;

namespace api.Data.DAO;

public class UserModel
{
    [Key]
    public int Id { get; set; }
    public required string KeycloakUuid { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
}
