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

        [Display(Name = "Email người dùng ")]
        public int? IdAc { get; set; }
        public virtual Account? Account { get; set; }

        [Display(Name = "Sự kiện ")]
        public int? IdEvent { get; set; }
        public virtual Event? IdEventNavigation { get; set; }

        [Display(Name = "Dự án ")]
        public int? IdProject { get; set; }


        public virtual Project? IdProjectNavigation { get; set; }

        [Display(Name = "Tác phẩm ")]
        public int? IdArtwork { get; set; }
        public virtual Artwork? IdArtworkNavigation { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Ảnh")]
        public string? UrlDocument { get; set; }

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
