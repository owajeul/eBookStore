using System.Linq.Expressions;

namespace eBookStore.Application.Interfaces;
public interface IRepository<T> where T : class
{
    Task Add(T entity);
    Task<T> Get(Expression<Func<T, bool>> filter);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    void Remove(T entity);
    Task Save();
    void Update(T entity);
}