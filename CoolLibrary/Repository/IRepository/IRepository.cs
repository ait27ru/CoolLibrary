using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoolLibrary.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<T> FindAsync(int id);

        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true);

        Task<T> FirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true);

        Task AddAsync(T entity);

        void Remove(T entity);

        Task SaveAsync();
    }
}
