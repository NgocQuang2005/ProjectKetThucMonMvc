using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ProjectParticipant
    {
        [Key]
        public int IdProjectParticipant { get; set; }

        [Required]
        [Display(Name = "Trạng Thái")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Dự Án")]
        public int IdProject { get; set; }

        [ForeignKey("IdProject")]
        public Project? Project { get; set; }

        [Required]
        [Display(Name = "Người Tham Gia")]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }

    }
}
