
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        public ProductRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Product>> GetListOnFilterAsync(Expression<Func<Product, bool>> filter = null, bool tracked = true)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            IQueryable<Product> products = context.Products
                .Include(p => p.Store)
                .Include(p => p.PriceHistory)
                .Include(p => p.Categories)
                .ThenInclude(p => p.Category);
            if (!tracked == true)
            {
                products = products.AsNoTracking();
            }
            if (filter != null)
            {
                products = products.Where(filter);
            }
            return await products.Take(10).Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                CountryOfOrigin = p.CountryOfOrigin,
                CurrentPrice = p.CurrentPrice,
                CurrentComparePrice = p.CurrentComparePrice,
                OriginalPrice = p.OriginalPrice,
                ComparePrice = p.ComparePrice,
                DiscountPercentage = p.DiscountPercentage,
                MemberDiscount = p.MemberDiscount,
                WasDiscount = p.WasDiscount,
                Unit = p.Unit,
                Size = p.Size,
                MaxQuantity = p.MaxQuantity,
                MinQuantity = p.MinQuantity,
                ImageUrl = p.ImageUrl,
                ProdCode = p.ProdCode,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Categories = p.Categories,
                StoreId = p.StoreId,
                Store = p.Store,
                PriceHistory = p.PriceHistory
                    .OrderByDescending(ph => ph.RecordedAt)
                    .Select(p => new PriceHistory
                    {
                        ProductId = p.ProductId,
                        Price = p.Price,
                        ComparePrice = p.ComparePrice,
                        CompareUnit = p.CompareUnit,
                        WasDiscount = p.WasDiscount,
                        RecordedAt = p.RecordedAt
                    }).ToList()
            }).ToListAsync();
        }

        public async Task<Product> GetOnFilterAsync(Expression<Func<Product, bool>> filter = null, bool tracked = true)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            IQueryable<Product> products = context.Products
                .Include(p => p.Store)
                .Include(p => p.PriceHistory)
                .Include(p => p.Categories);
            if (!tracked == true)
            {
                products = products.AsNoTracking();
            }
            if (filter != null)
            {
                products = products.Where(filter);
            }
            return await products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                CountryOfOrigin = p.CountryOfOrigin,
                CurrentPrice = p.CurrentPrice,
                CurrentComparePrice = p.CurrentComparePrice,
                OriginalPrice = p.OriginalPrice,
                ComparePrice = p.ComparePrice,
                DiscountPercentage = p.DiscountPercentage,
                MemberDiscount = p.MemberDiscount,
                WasDiscount = p.WasDiscount,
                Unit = p.Unit,
                Size = p.Size,
                MaxQuantity = p.MaxQuantity,
                MinQuantity = p.MinQuantity,
                ImageUrl = p.ImageUrl,
                ProdCode = p.ProdCode,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                StoreId = p.StoreId,
                Store = p.Store,
                Categories = p.Categories
                    .Select(c => new CategoryList
                    {
                        Id = c.Id,
                        Category = c.Category,
                        StoreId = c.StoreId,
                        ProductId = c.ProductId,
                    }).ToList(),
                PriceHistory = p.PriceHistory
                    .OrderByDescending(ph => ph.RecordedAt)
                    .Select(p => new PriceHistory
                    {
                        ProductId = p.ProductId,
                        Price = p.Price,
                        ComparePrice = p.ComparePrice,
                        CompareUnit = p.CompareUnit,
                        WasDiscount = p.WasDiscount,
                        RecordedAt = p.RecordedAt
                    }).ToList()
            }).FirstOrDefaultAsync();
        }

        public Task<Product> AddAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}