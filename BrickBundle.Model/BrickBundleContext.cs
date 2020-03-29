using Microsoft.EntityFrameworkCore;

namespace BrickBundle.Model
{
    /// <summary>
    /// Represents a session with the BrickBundle database.
    /// </summary>
    public class BrickBundleContext : DbContext
    {
        public static string ConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=BrickBundle;Trusted_Connection=True;";
        public static bool IsSqlite { get; set; } = false;

        internal DbSet<User> Users { get; set; }
        internal DbSet<Part> Parts { get; set; }
        internal DbSet<LegoColor> LegoColors { get; set; }
        internal DbSet<Category> Categories { get; set; }
        internal DbSet<UserPart> UserParts { get; set; }
        internal DbSet<Set> Sets { get; set; }
        internal DbSet<Inventory> Inventories { get; set; }
        internal DbSet<Theme> Themes { get; set; }
        internal DbSet<InventoryPart> InventoryParts { get; set; }
        internal DbSet<InventorySet> InventorySets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (IsSqlite)
            {
                optionsBuilder.UseSqlite(ConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PartConfiguration());
            modelBuilder.ApplyConfiguration(new LegoColorConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserPartConfiguration());
            modelBuilder.ApplyConfiguration(new SetConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryConfiguration());
            modelBuilder.ApplyConfiguration(new ThemeConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryPartConfiguration());
            modelBuilder.ApplyConfiguration(new InventorySetConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public static void EnsureDatabaseCreated()
        {
            using (var context = new BrickBundleContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
