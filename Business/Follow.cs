﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Follow
    {
        [Key]
        public int IdFollow { get; set; }

        [Required]
        [Display(Name ="Trạng Thái")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Người Theo Dõi")]
        public int IdFollower { get; set; }

        [ForeignKey("IdFollower")]
        public virtual Account? Follower { get; set; }

        [Required]
        [Display(Name = "Người Đang theo dõi")]
        public int IdFollowing { get; set; }

        [ForeignKey("IdFollowing")]
        public virtual Account? Following { get; set; }

        public DateTime? CreatedWhen { get; set; } = DateTime.Now;

        public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

    }
}
