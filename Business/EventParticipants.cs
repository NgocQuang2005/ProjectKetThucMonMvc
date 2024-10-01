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
        [Display(Name = "Trạng Thái")]
        public bool Active { get; set; }

        [Required]
        [Display(Name ="Sự Kiện")]
        public int IdEvent { get; set; }

        [ForeignKey("IdEvent")]
        public Event? Event { get; set; }

        [Required]
        [Display(Name = "Người Đăng")]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }
        [Display(Name = "Thời Gian Đăng Ký")]
        public DateTime RegistrationTime { get; set; }
    }
}
