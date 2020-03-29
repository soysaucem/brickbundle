using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrickBundle.Model
{
    internal class InventorySetConfiguration : IEntityTypeConfiguration<InventorySet>
    {
        public void Configure(EntityTypeBuilder<InventorySet> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => new { a.InventoryID, a.SetID }).IsUnique();
            builder.HasIndex(a => a.InventoryID);
            builder.HasOne(a => a.Inventory).WithMany(b => b.InventorySets).HasForeignKey(a => a.InventoryID).IsRequired();
            builder.HasOne(a => a.Set).WithMany(b => b.InventorySets).HasForeignKey(a => a.SetID).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class InventorySet
    {
        #region Properties
        public long ID { get; set; }
        public long InventoryID { get; set; }
        public Inventory Inventory { get; set; }
        public long SetID { get; set; }
        public Set Set { get; set; }
        public int Quantity { get; set; }
        #endregion
    }
}
