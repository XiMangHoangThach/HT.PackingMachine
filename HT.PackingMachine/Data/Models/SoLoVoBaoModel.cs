using HT.PackingMachine.Data.Entities;

namespace HT.PackingMachine.Data.Models
{
    public class SoLoVoBaoModel
    {
        public int Id { get; set; }
        public string BatchNum { get; set; }
        public int Category_Id { get; set; }
        public int? Qty { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }
        public Category Category { get; set; }
    }
}
