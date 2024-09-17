using Business;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ArtistSocialNetwork.Models
{
    public class DocumentInfoDTO
    {
        public int IdDcIf { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Active { get; set; }

        public int? IdAc { get; set; }
        [Display(Name = "Email người dùng ")]
        public virtual Account? Account { get; set; }

        public int? IdEvent { get; set; }
        [Display(Name = "Sự kiện ")]
        public virtual Event? IdEventNavigation { get; set; }

        public int? IdProject { get; set; }

        [Display(Name = "Dự án ")]

        public virtual Project? IdProjectNavigation { get; set; }

        public int? IdArtwork { get; set; }
        [Display(Name = "Tác phẩm ")]
        public virtual Artwork? IdArtworkNavigation { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? ImageFile { get; set; } = null!;
        [Display(Name = "Ảnh")]
        public string UrlDocument { get; set; }

        public int? Created_by { get; set; }
        [ForeignKey("Created_by")]
        public virtual Account? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Created_when { get; set; } = DateTime.Now;

        public int? Last_update_by { get; set; }
        [ForeignKey("Last_update_by")]
        public virtual Account? LastUpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Last_update_when { get; set; } = DateTime.Now;
    }
}
