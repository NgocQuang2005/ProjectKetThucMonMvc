using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AccountDTO
    {
        public int IdAccount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]

        public string Password { get; set; } // Mật khẩu sẽ được mã hóa (MD5)
        public int IdRole { get; set; }

    }
}
