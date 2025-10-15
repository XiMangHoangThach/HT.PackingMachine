using HT.PackingMachine.Components.Pages;
using HT.PackingMachine.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
namespace HT.PackingMachine.Data
{

    public class XhsContext : DbContext
    {
        public XhsContext(DbContextOptions<XhsContext> options)
            : base(options)
        {
        }
        public virtual DbSet<BatchNum_List> BatchNum_Lists { get; set; }
        public virtual DbSet<BatchNum_Register> BatchNum_Registers { get; set; }
        public virtual DbSet<Category> Category { get; set; }
 
        public virtual DbSet<Data.Entities.PackingMachine> PackingMachines { get; set; }
        public virtual DbSet<PackingMachineCategoryHistory> PackingMachineCategoryHistories { get; set; }


        [DbFunction("f_Vehicle_Code", "xhs")]
        public virtual string f_Vehicle_Code(string vehicleCode)
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "xhs");
       
            base.OnModelCreating(modelBuilder);
        }

      
    }
}
