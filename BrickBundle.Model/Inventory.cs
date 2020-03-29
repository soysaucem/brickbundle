using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace BrickBundle.Model
{
    internal class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => new { a.Version, a.SetID }).IsUnique();
            builder.HasIndex(a => a.SetID);
            builder.HasOne(a => a.Set).WithMany(b => b.Inventories).HasForeignKey(a => a.SetID).IsRequired();
        }
    }

    public class Inventory
    {
        #region Properties
        public long ID { get; set; }
        public long Version { get; set; }
        public long SetID { get; set; }
        public Set Set { get; set; }
        public ICollection<InventoryPart> InventoryParts { get; set; }
        public ICollection<InventorySet> InventorySets { get; set; }
        #endregion
    }
}
