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

        [Display(Name = "Tác Phẩm")]
        public int IdArtwork { get; set; }

        [ForeignKey("IdArtwork")]
        public Artwork? Artwork { get; set; }

        [Display(Name = "Nguời Dùng")]
        public int IdAc { get; set; }

        [ForeignKey("IdAc")]
        public Account? Account { get; set; }

        public bool Action { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;


    }
}
