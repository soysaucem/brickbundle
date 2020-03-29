using System;
using System.Threading.Tasks;

namespace BrickBundle.Model
{
    /// <summary>
    /// Abstract class containing commonly used DbContext operations.
    /// </summary>
    public abstract class IO
    {
        protected BrickBundleContext DbContext;

        #region Methods
        /// <summary>
        /// Returns the context this entity uses to communicate with the datbase.
        /// </summary>
        internal BrickBundleContext GetDbcontext()
        {
            return DbContext;
        }

        /// <summary>
        /// Sets the context this entity uses to communicate with the database.
        /// </summary>
        internal void SetDbcontext(BrickBundleContext context)
        {
            DbContext = context;
        }

        /// <summary>
        /// Returns whether the entity state is added.
        /// </summary>
        public bool IsNew()
        {
            var state = DbContext.Entry(this).State;
            return state == Microsoft.EntityFrameworkCore.EntityState.Added;
        }

        /// <summary>
        /// Returns whether the DbContext has changes.
        /// </summary>
        internal bool HasChanges()
        {
            return DbContext.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        internal async virtual Task<bool> SaveChangesAsync()
        {
            try
            {
                if (HasChanges())
                {
                    await DbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return false;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates a new database context for communicating with the database.
        /// </summary>
        internal static BrickBundleContext CreateDbContext()
        {
            return new BrickBundleContext();
        }
        
        /// <summary>
        /// Creates new entity tracked by a new database context.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        internal static async Task<T> CreateAsync<T>() where T : IO
        {
            var context = CreateDbContext();
            var entity = Activator.CreateInstance<T>();
            entity.DbContext = context;
            await context.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Returns entity in database tracked by a new database context.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="key">Entity's primary key</param>
        internal static async Task<T> FindAsync<T>(params object[] key) where T : IO
        {
            var context = CreateDbContext();
            if (!(await context.Set<T>().FindAsync(key) is IO entity))
            {
                return null;
            }
            entity.DbContext = context;
            return (T)entity;
        }
        #endregion
    }
}
