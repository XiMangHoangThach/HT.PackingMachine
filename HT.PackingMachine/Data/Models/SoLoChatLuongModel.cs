using HT.PackingMachine.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HT.PackingMachine.Data.Models
{
    public class SoLoChatLuongModel
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Chưa chọn chủng loại")]
        public int CategoryId { get; set; }
        [Required]
        public string BatchNum { get; set; } = string.Empty;
        [Required]
        public int? Qty { get; set; }

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }
        public string QualityNum { get; set; } = string.Empty;
        [Required(ErrorMessage = "Chưa chọn ngày chất lượng")]
        public DateTime? QualityDate { get; set; }
        public Category? Category { get; set; }
    }
}
