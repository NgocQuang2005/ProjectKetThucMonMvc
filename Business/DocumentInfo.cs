using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace Business
{
    public partial class DocumentInfo
    {
        [Key]
        public int IdDcIf { get; set; }

        [Required]
        public bool Active { get; set; }

        // Foreign Key - AccountDetail
        public int? IdAc { get; set; }
        [ForeignKey("IdAc")]
        public virtual Account? Account { get; set; }

        // Foreign Key - Event
        [Display(Name = "Sự Kiện")]

        public int? IdEvent { get; set; }
        [ForeignKey("IdEvent")]
        public virtual Event? IdEventNavigation { get; set; }

        // Foreign Key - Project
        public int? IdProject { get; set; }
        [ForeignKey("IdProject")]
        public virtual Project? IdProjectNavigation { get; set; }

        // Foreign Key - Artwork
        public int? IdArtwork { get; set; }
        [ForeignKey("IdArtwork")]
        public virtual Artwork? IdArtworkNavigation { get; set; }
        public string? UrlDocument { get; set; }

        // Foreign Key - Created By
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public int? Created_by { get; set; }
        [ForeignKey("Created_by")]
        public virtual Account? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Created_when { get; set; } = DateTime.Now;

        // Foreign Key - Last Updated By
        public int? Last_update_by { get; set; }
        [ForeignKey("Last_update_by")]
        public virtual Account? LastUpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Last_update_when { get; set; } = DateTime.Now;

    }
}
