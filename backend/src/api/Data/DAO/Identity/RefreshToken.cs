using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.DAO.Identity;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string JwtId { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateExpire { get; set; }
    public required string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public UserModel? User { get; set; }
}
