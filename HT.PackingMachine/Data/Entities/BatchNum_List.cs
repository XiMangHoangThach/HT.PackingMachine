using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.PackingMachine.Data.Entities
{
    [Table("BatchNum_List")]
    public class BatchNum_List
    {
        public int Id { get; set; }
        public string BatchNum { get; set; }
        public int Category_Id { get; set; }
        public int? Qty { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }
        public string? QualityNum { get; set; }
        public DateTime? QualityDate { get; set; }
        [ForeignKey(nameof(Category_Id))]
        public Category Category { get; set; }
    }

}
