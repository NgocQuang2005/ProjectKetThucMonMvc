using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Business
{
    public partial class DocumentInfo
    {
        [Key]
        public int IdDcIf { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public bool Active { get; set; }

        [Display(Name = "Tài khoản")]
        public int? IdAc { get; set; }

        [ForeignKey("IdAc")]
        public virtual Account? Account { get; set; }

        [Display(Name = "Sự Kiện")]
        public int? IdEvent { get; set; }

        [ForeignKey("IdEvent")]
        public virtual Event? IdEventNavigation { get; set; }

        [Display(Name = "Dự án")]
        public int? IdProject { get; set; }

        [ForeignKey("IdProject")]
        public virtual Project? IdProjectNavigation { get; set; }

        [Display(Name = "Tác phẩm")]
        public int? IdArtwork { get; set; }

        [ForeignKey("IdArtwork")]
        public virtual Artwork? IdArtworkNavigation { get; set; }

        [Display(Name = "Ảnh")]
        public string? UrlDocument { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

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
