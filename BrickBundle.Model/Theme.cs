using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace BrickBundle.Model
{
    internal class ThemeConfiguration : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.Name);
            builder.HasOne(a => a.Parent).WithMany(b => b.Children).HasForeignKey(a => a.ParentID);
        }
    }

    public class Theme
    {
        #region Properties
        public long ID { get; set; }
        public string Name { get; set; }
        public long? ParentID { get; set; }
        public Theme Parent { get; set; }
        public ICollection<Set> Sets { get; set; }
        public ICollection<Theme> Children { get; set; }
        #endregion
    }
}
