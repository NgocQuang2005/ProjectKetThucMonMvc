using Business;
using System.ComponentModel.DataAnnotations;

namespace ArtistSocialNetwork.Models
{
    public class AccountDTO
    {
        public int IdAccount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; } // Cho phép Password là null khi cập nhật

        public int IdRole { get; set; }

        [Display(Name = "Vai trò")]
        public Role? AccountRole { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public string? ProfileImage { get; set; } // Nếu có thuộc tính ảnh đại diện
        public string? ProfileImageUrl { get; set; } // Để hiển thị URL của ảnh đại diện
    }
}
