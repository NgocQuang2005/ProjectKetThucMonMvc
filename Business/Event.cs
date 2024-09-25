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
        [Display(Name = "Trạng Thái")]
        [Required]
        public bool Active { get; set; }

        [Display(Name = "Tiêu đề")]
        [MaxLength(255)]
        public string? Title { get; set; }

        [Display(Name = "Người Đăng")]
        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }

        [Display(Name = "Nội dung")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [Required]
        public DateTime EndDate { get; set; }
        [Display(Name = "Số người")]
        [Required]
        public int NumberOfPeople { get; set; }

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account? Creator { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account? Updater { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public virtual ICollection<DocumentInfo>? DocumentInfos { get; set; }
        public ICollection<EventParticipants>? EventParticipants { get; set; }

    }
}
