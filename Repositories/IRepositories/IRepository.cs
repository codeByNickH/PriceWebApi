using System.Linq.Expressions;

namespace PriceWebApi.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetListOnFilterAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task<T> GetOnFilterAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}