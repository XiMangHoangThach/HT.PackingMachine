using System.ComponentModel.DataAnnotations.Schema;

namespace HT.PackingMachine.Data.Entities
{
    [Table("PackingMachine_Category_History")]
    public class PackingMachineCategoryHistory
    {
        public int Id { get; set; }
        public int PackingMachine_Id { get; set; }
        public int Category_Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }
        [ForeignKey(nameof(Category_Id))]
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(PackingMachine_Id))]
        public virtual PackingMachine PackingMachine { get; set; }
    }
}
