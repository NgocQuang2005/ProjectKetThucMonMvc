using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class EventParticipants
    {
        [Key]
        public int IdEventParticipant { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public int IdEvent { get; set; }

        [ForeignKey("IdEvent")]
        public Event Event { get; set; }

        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account Account { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}
