using System.ComponentModel.DataAnnotations;

namespace api.Data.DTO;

public class UserDTO
{
    [Required]
    public required string KeycloakUuid { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
