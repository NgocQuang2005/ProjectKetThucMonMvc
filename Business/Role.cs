using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        [MaxLength(255)]
        public string RoleName { get; set; }

        public ICollection<Account>? RoleAccount { get; set; }

    }
}
