using System.ComponentModel.DataAnnotations;

namespace api.Data.DTO;

public class TokenRequestDTO
{

    [Required]
    public string Token { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}
