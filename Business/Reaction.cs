using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Reaction
    {
        [Key]
        public int IdReaction { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public int IdArtwork { get; set; }

        [ForeignKey("IdArtwork")]
        public Artwork Artwork { get; set; }

        [Required]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account Account { get; set; }

        public string Action { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

    }
}
