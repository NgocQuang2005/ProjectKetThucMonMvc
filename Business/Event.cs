using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Event
    {
        [Key]
        public int IdEvent { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account Account { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int NumberOfPeople { get; set; }

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account Creator { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account Updater { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public ICollection<DocumentInfo> DocumentInfos { get; set; }
        public ICollection<EventParticipants> EventParticipants { get; set; }

    }
}
