using System.Linq.Expressions;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;

namespace PriceWebApi.Repositories.IRepositories
{
    public interface IStoreLocationRepository
    {
        Task<List<StoreLocationDTO>> GetListOnFilterAndPageAsync(Expression<Func<StoreLocation, bool>> filter = null, int pageNumber = 0, int pageSize = 3);
        Task<List<StoreLocationDTO>> GetListWithFilterAsync(Expression<Func<StoreLocation, bool>> locationFilter = null, Expression<Func<Product, bool>> productFilter = null, int pageNumber = 0, int pageSize = 10);
    }
}