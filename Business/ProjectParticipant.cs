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
        public bool Active { get; set; }

        [Required]
        public int IdProject { get; set; }

        [ForeignKey("IdProject")]
        public Project Project { get; set; }

        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account Account { get; set; }

    }
}
