using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class StoreRepository : IRepository<Store>
    {
        private readonly AppDbContext _dbContext;
        public StoreRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Store>> GetListOnFilterAsync(Expression<Func<Store, bool>> filter = null, bool tracked = true)
        {
            IQueryable<Store> products = _dbContext.Stores
                .Include(x => x.Products);
            //  .Include(x => x.StoreLocation)
            if (!tracked == true)
            {
                products = products.AsNoTracking();
            }
            if (filter != null)
            {
                products = products.Where(filter);
            }
            return await products.ToListAsync();
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