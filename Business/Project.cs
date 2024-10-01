using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Project
    {
        [Key]
        public int IdProject { get; set; }

        [Required]
        [Display(Name = "Trạng Thái")]
        public bool Active { get; set; }

        [MaxLength(255)]
        [Display(Name = "Tiêu Đề")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Người Đăng")]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }
        [Display(Name = "Nội Dung")]
        public string Description { get; set; }
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account? Creator { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account? Updater { get; set; }

        public ICollection<DocumentInfo>? DocumentInfos { get; set; }
        public ICollection<ProjectParticipant>? ProjectParticipants { get; set; }
    }
}
