using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrickBundle.Model
{
    internal class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.Code).IsUnique();
            builder.HasIndex(a => a.Name);
            builder.HasOne(a => a.Category).WithMany(b => b.Parts).HasForeignKey(a => a.CategoryID).IsRequired();
        }
    }

    public class Part
    {
        #region Properties
        public long ID { get; set; }
        public long CategoryID { get; set; }
        public Category Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ICollection<UserPart> UserParts { get; set; }
        public ICollection<InventoryPart> InventoryParts { get; set; }
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns a all parts categories and colors in the database.
        /// </summary>
        public static async Task<DTO.ListPartsCategoriesColorsDTO> ListPartsCategoriesColors()
        {
            using (var context = IO.CreateDbContext())
            {
                var partsTask = context.Parts
                    .Select(a => new DTO.PartListItemDTO()
                    {
                        ID = a.ID,
                        Code = a.Code,
                        Name = a.Name,
                        Category = a.Category.Name
                    })
                    .ToArrayAsync();

                var colorsTask = context.LegoColors
                    .Select(a => new DTO.ColorListItemDTO()
                    {
                        ID = a.ID,
                        Name = a.Name,
                    })
                    .ToArrayAsync();

                var categoriesTask = context.Categories.Select(a => a.Name).ToArrayAsync();

                await Task.WhenAll(partsTask, categoriesTask, colorsTask);
                return new DTO.ListPartsCategoriesColorsDTO()
                {
                    Categories = categoriesTask.Result,
                    Colors = colorsTask.Result,
                    Parts = partsTask.Result
                };
            }
        }

        /// <summary>
        /// Lists parts with corresponding codes.
        /// </summary>
        public static async Task<DTO.PartListItemDTO[]> ListPartsByCode(IEnumerable<string> codes)
        {
            using (var context = IO.CreateDbContext())
            {
                return await context.Parts
                    .Where(a => codes.Contains(a.Code))
                    .Select(a => new DTO.PartListItemDTO()
                    {
                        ID = a.ID,
                        Code = a.Code,
                        Name = a.Name,
                        Category = a.Category.Name
                    })
                    .ToArrayAsync();
            }
        }
        #endregion
    }
}
