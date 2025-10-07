using System.Linq.Expressions;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;

namespace PriceWebApi.Repositories.IRepositories
{
    public interface IStoreLocationRepository
    {
        Task<List<StoreLocationDTO>> GetListOnFilterAndPageAsync(Expression<Func<StoreLocation, bool>> filter = null, int pageNumber = 0, int pageSize = 10);
        Task<List<StoreLocationDTO>> GetListWithFilterAsync(Expression<Func<StoreLocation, bool>> locationFilter = null, Expression<Func<Product, bool>> productFilter = null, string sortBy = "discount", int pageNumber = 0, int pageSize = 10);
        Task<List<StoreLocationDTO>> GetListWithSearchAsync(Expression<Func<StoreLocation, bool>> locationFilter = null, string searchString = "", string sortBy = "discount", int pageNumber = 0, int pageSize = 10);

    }
}