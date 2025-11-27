using System.Linq.Expressions;

namespace WorkflowApi.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Basic CRUD
        Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        // Query Methods
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
        
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
        
        Task<int> CountAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);

        // Advanced Query
        IQueryable<T> Query();
        
        // Batch Operations (Performance)
        Task AddRangeAsync(
            IEnumerable<T> entities, 
            CancellationToken cancellationToken = default);
        
        Task UpdateRangeAsync(
            IEnumerable<T> entities, 
            CancellationToken cancellationToken = default);
        
        Task DeleteRangeAsync(
            IEnumerable<T> entities, 
            CancellationToken cancellationToken = default);
    }
}