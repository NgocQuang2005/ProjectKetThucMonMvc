using Business;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Project
{
    [Key]
    public int IdProject { get; set; }

    [Required]
    public bool Active { get; set; }

    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public int IdAc { get; set; }

    [ForeignKey("IdAc")]
    public Account Account { get; set; }

    public string Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime? CreatedWhen { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public Account Creator { get; set; }

    public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;

    public int? LastUpdateBy { get; set; }

    [ForeignKey("LastUpdateBy")]
    public Account Updater { get; set; }

    public ICollection<DocumentInfo> DocumentInfos { get; set; }
    public ICollection<ProjectParticipant> ProjectParticipants { get; set; }
}
