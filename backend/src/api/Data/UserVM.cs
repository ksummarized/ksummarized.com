using System.ComponentModel.DataAnnotations;

namespace api.Data
{
    public class UserVM
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password{ get; set; }
    }
}
