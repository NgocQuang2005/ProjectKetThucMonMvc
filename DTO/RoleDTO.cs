using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RoleDTO
    {
        public int IdRole { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Active { get; set; }
        [Display(Name = "Tên vai trò")]
        [Required(ErrorMessage = "Vui lòng nhập tên vai trò")]
        public string RoleName { get; set; }
    }
}
