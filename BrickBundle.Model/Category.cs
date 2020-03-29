using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace BrickBundle.Model
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.Name).IsUnique();
        }
    }

    public class Category
    {
        #region Properties
        public long ID { get; set; }
        public string Name { get; set; }
        public ICollection<Part> Parts { get; set; }
        #endregion
    }
}
