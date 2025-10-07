using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class StoreRepository : IRepository<Store>
    {
        
        public StoreRepository()
        {

        }

        public async Task<List<Store>> GetListOnFilterAsync(Expression<Func<Store, bool>> filter = null, bool tracked = true)
        {
            
            throw new NotImplementedException();
        }

        public Task<Store> GetOnFilterAsync(Expression<Func<Store, bool>> filter = null, bool tracked = true)
        {
            throw new NotImplementedException();
        }

        public Task<Store> AddAsync(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task<Store> UpdateAsync(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}