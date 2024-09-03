﻿using System.ComponentModel.DataAnnotations;

namespace ArtistSocialNetwork.Models
{
    public class SignUpWeb
    {
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string Password { get; set; }
        public bool Active { get; set; } = true;
        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ")]
        [MaxLength(255)]
        public string Fullname { get; set; }
        public int IdRole { get; set; } = 2;
        [Display(Name = "Ngày sinh")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Quốc gia")]
        [Required(ErrorMessage = "Vui lòng nhập Quốc Gia")]
        [MaxLength(255)]
        public string Nationality { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [MaxLength(10)]
        public string Gender { get; set; }
    }
}
