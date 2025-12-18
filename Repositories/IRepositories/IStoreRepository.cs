using System.Linq.Expressions;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;

namespace PriceWebApi.Repositories.IRepositories
{
    public interface IStoreRepository
    {
        Task<StoreDTO> GetSingleStoreListWithFilterAsync(int? storeId = 0, Expression<Func<Product, bool>> productFilter = null, string sortBy = null, int pageNumber = 1, int pageSize = 10);
        Task<StoreDTO> GetSingleStoreListWithSearchAsync(int? storeId = 0, string searchString = null, string sortBy = null, int pageNumber = 1, int pageSize = 10);
        Task<StoreDTO> GetSingleStoreListFirstPageAsync(int? storeId = 0, string sortBy = null, int pageNumber = 1, int pageSize = 10);
    }
}