using System.ComponentModel.DataAnnotations;

namespace ArtistSocialNetwork.Models
{
    public class LoginWeb
    {
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
