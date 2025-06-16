using System.Linq.Expressions;

namespace HR_Operations_System.Business
{
    public interface IRepository
    {
        Task<IQueryable<T>> GetAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(object id) where T : class;
        Task<T> GetByAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task UpdateAsync<T>(object id, Func<T, Task> updateFn) where T : class;
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task SaveAsync();
    }



}
