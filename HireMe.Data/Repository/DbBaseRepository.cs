namespace HireMe.Data.Repository
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using HireMe.Data.Repository.Interfaces;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using HireMe.Core.Helpers;

    public class DbBaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public DbBaseRepository(BaseDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet = this.context.Set<TEntity>();
        }

        protected DbSet<TEntity> dbSet { get; set; }
        protected BaseDbContext context { get; set; }

        public async Task<OperationResult> AddAsync(TEntity entity)
        {
            await this.dbSet.AddAsync(entity);

            return await SaveChangesAsync();
        }

        public async Task<OperationResult> AddRangeAsync(TEntity entity)
        {
            await this.dbSet.AddRangeAsync(entity);
            var result = await SaveChangesAsync();
            return result;
        }
        public IQueryable<TEntity> Set()
        {
            return this.dbSet;
        }
        public IAsyncEnumerable<TEntity> SetAsync()
        {
            return this.dbSet.AsAsyncEnumerable();
        }
        public async Task<OperationResult> DeleteAsync(TEntity entity)
        {
            this.dbSet.Remove(entity);
            var result = await SaveChangesAsync();
            return result;
        }

        public async Task<OperationResult> UpdateAsync(TEntity entity)
        {
            var entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
            var result = await SaveChangesAsync();
            return result;
        }

        public async Task<OperationResult> SaveChangesAsync()
        {
            var success = await this.context.SaveChangesAsync() > 0;
            return success ? OperationResult.SuccessResult() : OperationResult.FailureResult("Changes saving failure!");
        }

        // GetByIdAsync: If you have a lot of data for example you have to load about 10000 entries,
        //from database then this method it will be the right choice(You can also use bulk operation).

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var ent = await this.dbSet.FindAsync(id);
            return ent;
        }

        public TEntity GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.context?.Dispose();
            }
        }
    }
}
