using System.ComponentModel.DataAnnotations;

namespace api.Data
{
    public class TokenRequestDTO
    {

        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
