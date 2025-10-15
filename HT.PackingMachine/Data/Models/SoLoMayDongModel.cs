using HT.PackingMachine.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HT.PackingMachine.Data.Models
{
    public class SoLoMayDongModel
    {
        public int Id { get; set; }
        public int BatchNum_List_Id { get; set; }
        public int? PackingMachine_Id { get; set; }
        public int? Qty { get; set; }
        public int? OrderNum { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte Status { get; set; }

        [ForeignKey(nameof(PackingMachine_Id))]
        public Data.Entities.PackingMachine PackingMachine { get; set; }
        [ForeignKey(nameof(BatchNum_List_Id))]
        public BatchNum_List BatchNum_List { get; set; }
    }
}
