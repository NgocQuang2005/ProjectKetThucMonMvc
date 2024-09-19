using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Account
    {
        [Key]
        public int IdAccount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(50)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(255)]
        public string Password { get; set; } // Mật khẩu sẽ được mã hóa (MD5)
        public int IdRole { get; set; }
        [ForeignKey("IdRole")]
        [Display(Name = "Vai trò")]
        public Role? AccountRole { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account? Creator { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account? Updater { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;
        
        public AccountDetail? AccountDetail { get; set; }
        public ICollection<AccountDetail>? CreatedAd { get; set; }
        public ICollection<AccountDetail>? UpdatedAd { get; set; }
        public ICollection<Artwork>? Artworks { get; set; }
        public ICollection<Artwork>? CreatedArt { get; set; }
        public ICollection<Artwork>? UpdatedArt { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Reaction>? Reactions { get; set; }
        public ICollection<Event>? Events { get; set; }
        public ICollection<Event>? CreatedE { get; set; }
        public ICollection<Event>? UpdatedE { get; set; }
        public ICollection<EventParticipants>? EPAccDt { get; set; }
        public ICollection<Follow>? Followers { get; set; }
        public ICollection<Follow>? Following { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<Project>? CreatedPj { get; set; }
        public ICollection<Project>? UpdatedPj { get; set; }
        public ICollection<ProjectParticipant>? PjAccDt { get; set; }
        public ICollection<DocumentInfo>? DocumentInfos { get; set; }

        public ICollection<DocumentInfo>? CreatedDocuments { get; set; }
        public ICollection<DocumentInfo>? UpdatedDocuments { get; set; }
    }
}
