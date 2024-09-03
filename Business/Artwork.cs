using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Business
{
    public class Artwork
    {
        [Key]
        public int IdArtwork { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account Account { get; set; }

        [Required]
        public int IdTypeOfArtwork { get; set; }

        [ForeignKey("IdTypeOfArtwork")]
        public TypeOfArtwork TypeOfArtwork { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Tags { get; set; }

        [Required]
        [MaxLength(50)]
        public string MediaType { get; set; }

        public string MediaUrl { get; set; }

        public int? Watched { get; set; }

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Account Creator { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public int? LastUpdateBy { get; set; }

        [ForeignKey("LastUpdateBy")]
        public Account? Updater { get; set; }

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<DocumentInfo> DocumentInfos { get; set; }

    }
}
