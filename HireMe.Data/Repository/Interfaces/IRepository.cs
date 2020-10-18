using HireMe.Core.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Data.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Set();

        IAsyncEnumerable<TEntity> SetAsync();

        Task<OperationResult> AddAsync(TEntity entity);

        Task<OperationResult> AddRangeAsync(TEntity entity);

        Task<OperationResult> UpdateAsync(TEntity entity);

        Task<OperationResult> DeleteAsync(TEntity entity);

        Task<OperationResult> SaveChangesAsync();

        Task<TEntity> GetByIdAsync(int id);

        TEntity GetById(int id);

        void Dispose();
    }
}
