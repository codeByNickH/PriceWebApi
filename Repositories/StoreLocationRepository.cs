using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class StoreLocationRepository : IStoreLocationRepository, IRepository<StoreLocation>
    {
        private readonly AppDbContext _dbContext;
        public StoreLocationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<StoreLocationDTO>> GetListOnFilterAndPageAsync(Expression<Func<StoreLocation, bool>> filter = null, int pageNumber = 0, int pageSize = 3)
        {
            IQueryable<StoreLocation> products = _dbContext.StoreLocations
                .Include(x => x.Stores)
                .ThenInclude(x => x.Products)
                .ThenInclude(x => x.Categories)
                .ThenInclude(x => x.Category);

            if (filter != null)
            {
                products = products.Where(filter);
            }

            return await products.Select(l => new StoreLocationDTO
            {
                Id = l.Id,
                City = l.City,
                District = l.District,
                Address = l.Address,
                PostalCode = l.PostalCode,
                Stores = l.Stores.Select(s => new StoreDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Products = s.Products
                        .OrderBy(p => p.WasDiscount ? 0 : 1)
                        // .ThenByDescending(p => p.DiscountPercentage)
                        .Skip(pageNumber * pageSize)
                        .Take(pageSize)
                        .Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Brand = p.Brand,
                            ComparePrice = p.ComparePrice,
                            CountryOfOrigin = p.CountryOfOrigin,
                            CreatedAt = p.CreatedAt,
                            CurrentComparePrice = p.CurrentComparePrice,
                            CurrentPrice = p.CurrentPrice,
                            DiscountPercentage = p.DiscountPercentage,
                            ImageUrl = p.ImageUrl,
                            MaxQuantity = p.MaxQuantity,
                            MemberDiscount = p.MemberDiscount,
                            MinQuantity = p.MinQuantity,
                            Name = p.Name,
                            OriginalPrice = p.OriginalPrice,
                            Size = p.Size,
                            Unit = p.Unit,
                            UpdatedAt = p.UpdatedAt,
                            WasDiscount = p.WasDiscount,
                            Categories = p.Categories.Select(c => new CategoryDTO
                            {
                                Id = c.Category.Id,
                                Name = c.Category.Name
                            }).ToList(),
                        }).ToList()
                }).ToList()
            }).AsNoTracking().ToListAsync();
                
        }
        public async Task<List<StoreLocationDTO>> GetListWithFilterAsync(Expression<Func<StoreLocation, bool>> locationFilter = null, Expression<Func<Product, bool>> productFilter = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<StoreLocation> products = _dbContext.StoreLocations
                .Include(x => x.Stores)
                .ThenInclude(x => x.Products)
                .ThenInclude(x => x.Categories)
                .ThenInclude(x => x.Category);

            if (locationFilter != null)
            {
                products = products.Where(locationFilter);
            }

            return await products.Select(l => new StoreLocationDTO
            {
                Id = l.Id,
                City = l.City,
                District = l.District,
                Address = l.Address,
                PostalCode = l.PostalCode,
                Stores = l.Stores.Select(s => new StoreDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Products = s.Products
                        .AsQueryable()
                        .Where(productFilter ?? (p => true))
                        .OrderBy(p => p.WasDiscount ? 0 : 1)
                        // .ThenByDescending(p => p.DiscountPercentage)
                        .Skip(pageNumber * pageSize)
                        .Take(pageSize)
                        .Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Brand = p.Brand,
                            ComparePrice = p.ComparePrice,
                            CountryOfOrigin = p.CountryOfOrigin,
                            CreatedAt = p.CreatedAt,
                            CurrentComparePrice = p.CurrentComparePrice,
                            CurrentPrice = p.CurrentPrice,
                            DiscountPercentage = p.DiscountPercentage,
                            ImageUrl = p.ImageUrl,
                            MaxQuantity = p.MaxQuantity,
                            MemberDiscount = p.MemberDiscount,
                            MinQuantity = p.MinQuantity,
                            Name = p.Name,
                            OriginalPrice = p.OriginalPrice,
                            Size = p.Size,
                            Unit = p.Unit,
                            UpdatedAt = p.UpdatedAt,
                            WasDiscount = p.WasDiscount,
                            Categories = p.Categories.Select(c => new CategoryDTO
                            {
                                Id = c.Category.Id,
                                Name = c.Category.Name
                            }).ToList(),
                        }).ToList()
                }).ToList()
            }).AsNoTracking().ToListAsync();
        }

        public Task<StoreLocation> GetOnFilterAsync(Expression<Func<StoreLocation, bool>> filter = null, bool tracked = true)
        {
            throw new NotImplementedException();
        }

        public Task<StoreLocation> AddAsync(StoreLocation entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<StoreLocation> UpdateAsync(StoreLocation entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<StoreLocation>> GetListOnFilterAsync(Expression<Func<StoreLocation, bool>> filter = null, bool tracked = true)
        {
            throw new NotImplementedException();
        }

    }
}