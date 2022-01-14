using System.ComponentModel.DataAnnotations;

namespace api.Data.DTO
{
    public class UserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
