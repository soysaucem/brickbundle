using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace BrickBundle.Model
{
    internal class LegoColorConfiguration : IEntityTypeConfiguration<LegoColor>
    {
        public void Configure(EntityTypeBuilder<LegoColor> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.Name).IsUnique();
        }
    }

    public class LegoColor
    {
        #region Properties
        public long ID { get; set; }
        public bool IsTransparent { get; set; }
        public string Name { get; set; }
        public string RGB { get; set; }
        public ICollection<UserPart> UserParts { get; set; }
        public ICollection<InventoryPart> InventoryParts { get; set; }
        #endregion
    }
}
