using Business;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AccountDetail
{
    [Key]
    public int IdAccountDt { get; set; }

    [Display(Name ="Trạng Thái")]
    [Required(ErrorMessage ="Vui lòng chọn trạng thái")]
    public bool Active { get; set; }

    [Display(Name ="Họ và Tên")]
    [Required(ErrorMessage ="Vui lòng nhập tên đầy đủ")]
    [MaxLength(255)]
    public string? Fullname { get; set; }
    public int IdAccount { get; set; }
    [ForeignKey("IdAccount")]
    public Account? account { get; set; }
    [Required]
    [Range(100000000, 999999999999, ErrorMessage = "CCCD phải là số và có độ dài từ 9 đến 12 ký tự.")]
    public long? CCCD { get; set; }  // Sử dụng long thay vì int
    public string? Description { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime? Birthday { get; set; }
    [Display(Name ="Quốc gia")]
    [Required(ErrorMessage ="Vui lòng nhập Quốc Gia")]
    [MaxLength(255)]
    public string? Nationality { get; set; }
    [Display(Name = "Giới tính")]
    [MaxLength(10)]
    public string? Gender { get; set; }
    [Display(Name = "Địa chỉ")]
    [MaxLength(500)]
    public string? Address { get; set; }
    public int? CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public Account? Creator { get; set; }
    public DateTime? CreatedWhen { get; set; } = DateTime.Now;
    public int? LastUpdateBy { get; set; }

    [ForeignKey("LastUpdateBy")]
    public Account? Updator { get; set; }
    public DateTime? LastUpdateWhen { get; set; } = DateTime.Now;
}
