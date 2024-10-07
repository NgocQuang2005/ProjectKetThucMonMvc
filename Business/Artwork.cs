using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Business
{
    public class Artwork
    {
        [Key]
        public int IdArtwork { get; set; }

        [Required]
        [Display(Name = "Trạng Thái")]
        public bool Active { get; set; }

        [Display(Name = "Tài khoản")]
        public int? IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }

        [Display(Name = "Loại Tác Phẩm")]
        public int? IdTypeOfArtwork { get; set; }

        [ForeignKey("IdTypeOfArtwork")]
        public TypeOfArtwork? TypeOfArtwork { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Tiêu đề")]
        public string? Title { get; set; }

        [Display(Name = "Miêu tả")]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Tags { get; set; } // Cho phép null

        [MaxLength(50)]
        public string? MediaType { get; set; } // Cho phép null

        public string? MediaUrl { get; set; } // Cho phép null

        [Display(Name = "Lượt xem")]
        public int? Watched { get; set; }

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account? Creator { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account? Updater { get; set; }   

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Reaction>? Reactions { get; set; }
        public virtual ICollection<DocumentInfo>? DocumentInfos { get; set; }
    }
}
