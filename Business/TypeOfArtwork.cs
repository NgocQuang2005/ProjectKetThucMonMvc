using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class TypeOfArtwork
    {
        [Key]
        public int IdTypeOfArtwork { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Tên loại tác phẩm")]

        public string NameTypeOfArtwork { get; set; }

        [Display(Name = "Miêu tả")]

        public string Description { get; set; }

        public ICollection<Artwork>? Artworks { get; set; }

    }
}
