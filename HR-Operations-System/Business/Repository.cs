using HR_Operations_System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HR_Operations_System.Business
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _db;

        public Repository(AppDbContext db)
        {
            _db = db;
        }

        public Task<IQueryable<T>> GetAsync<T>() where T : class
        {
            return Task.FromResult(_db.Set<T>().AsQueryable());
        }

        public async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _db.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _db.Set<T>().AddAsync(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync<T>(object id, Func<T, Task> updateFn) where T : class
        {
            var dbSet = _db.Set<T>();
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"{typeof(T).Name} with ID {id} not found.");
            }

            await updateFn(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var dbSet = _db.Set<T>();
            var entity = await dbSet.FirstOrDefaultAsync(predicate);
            if (entity == null)
            {
                throw new Exception($"{typeof(T).Name} not found for the given condition.");
            }

            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }

}
