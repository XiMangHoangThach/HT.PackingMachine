using System.ComponentModel.DataAnnotations.Schema;

namespace HT.PackingMachine.Data.Entities
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descr { get; set; }
        public int? PackingType { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }

        public virtual ICollection<PackingMachineCategoryHistory> PackingMachineCategoryHistory { get; set; }
        public virtual ICollection<BatchNum_List> BatchNum_Lists { get; set; }
    }
}
