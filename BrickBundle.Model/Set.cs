using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace BrickBundle.Model
{
    internal class SetConfiguration : IEntityTypeConfiguration<Set>
    {
        public void Configure(EntityTypeBuilder<Set> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.SetNum).IsUnique();
            builder.HasOne(a => a.Theme).WithMany(b => b.Sets).HasForeignKey(a => a.ThemeID).IsRequired();
        }
    }

    public class Set
    {
        #region Properties
        public long ID { get; set; }
        public string SetNum { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public long ThemeID { get; set; }
        public Theme Theme { get; set; }
        public int NumParts { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<InventorySet> InventorySets { get; set; }
        #endregion
    }
}
