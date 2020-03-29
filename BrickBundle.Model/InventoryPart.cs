using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrickBundle.Model
{
    internal class InventoryPartConfiguration : IEntityTypeConfiguration<InventoryPart>
    {
        public void Configure(EntityTypeBuilder<InventoryPart> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => new { a.InventoryID, a.PartID, a.ColorID, a.IsSpare }).IsUnique();
            builder.HasIndex(a => a.InventoryID);
            builder.HasOne(a => a.Inventory).WithMany(b => b.InventoryParts).HasForeignKey(a => a.InventoryID).IsRequired();
            builder.HasOne(a => a.Part).WithMany(b => b.InventoryParts).HasForeignKey(a => a.PartID).IsRequired();
            builder.HasOne(a => a.Color).WithMany(b => b.InventoryParts).HasForeignKey(a => a.ColorID).IsRequired();
        }
    }


    public class InventoryPart
    {
        #region Properties
        public long ID { get; set; }
        public long InventoryID { get; set; }
        public Inventory Inventory { get; set; }
        public long PartID { get; set; }
        public Part Part { get; set; }
        public long ColorID { get; set; }
        public LegoColor Color { get; set; }
        public int Quantity { get; set; }
        public bool IsSpare { get; set; }
        #endregion
    }
}
