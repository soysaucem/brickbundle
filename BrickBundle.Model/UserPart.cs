using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrickBundle.Model
{
    internal class UserPartConfiguration : IEntityTypeConfiguration<UserPart>
    {
        public void Configure(EntityTypeBuilder<UserPart> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => new { a.UserID, a.PartID, a.ColorID }).IsUnique();
            builder.HasIndex(a => a.UserID);
            builder.HasIndex(a => a.PartID);
            builder.HasIndex(a => a.Quantity);
            builder.HasOne(a => a.User).WithMany(b => b.UserParts).HasForeignKey(a => a.UserID).IsRequired();
            builder.HasOne(a => a.Part).WithMany(b => b.UserParts).HasForeignKey(a => a.PartID).IsRequired();
            builder.HasOne(a => a.Color).WithMany(b => b.UserParts).HasForeignKey(a => a.ColorID).IsRequired();
        }
    }

    public class UserPart
    {
        #region Properties
        public long ID { get; set; }
        public long UserID { get; set; }
        public User User { get; set; }
        public long PartID { get; set; }
        public Part Part { get; set; }
        public long ColorID { get; set; }
        public LegoColor Color { get; set; }
        public int Quantity { get; set; }
        #endregion
    }
}