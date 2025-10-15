using System.ComponentModel.DataAnnotations.Schema;

namespace HT.PackingMachine.Data.Entities
{
    [Table("PackingMachine")]
    public class PackingMachine
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descr { get; set; }
        public int? RegionId { get; set; }
        public int PackingType { get; set; }
        //public int RegionId { get; set; }
        public byte Status { get; set; }
    }
}
