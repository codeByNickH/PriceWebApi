using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        public StoreRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // Use this for pagination when only one store is needed, get on store id
        public async Task<StoreDTO> GetSingleStoreListWithFilterAsync(int? storeId = 0, Expression<Func<Product, bool>> productFilter = null, string sortBy = "discount", int pageNumber = 1, int pageSize = 10)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var query = context.Products
                .Include(p => p.Categories)
                .ThenInclude(p => p.Category)
                .Where(p => p.StoreId == storeId)
                .Where(productFilter);

            query = sortBy?.ToLowerInvariant() switch
            {
                "lowprice" => query.OrderBy(p => p.CurrentPrice),
                "highprice" => query.OrderByDescending(p => p.CurrentPrice),
                "lowcompareprice" => query.OrderBy(p => p.CurrentComparePrice),
                "highcompareprice" => query.OrderByDescending(p => p.CurrentComparePrice),
                "discountcompareprice" => query.OrderBy(p => p.WasDiscount ? 0 : 1).ThenBy(p => p.CurrentComparePrice),
                "discountpercentage" => query.OrderByDescending(p => p.DiscountPercentage),
                _ => query.OrderBy(p => p.WasDiscount ? 0 : 1).ThenBy(p => p.CurrentPrice)
            };

            var product = await query

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
                })
                .AsNoTracking()
                .ToListAsync();

            System.Console.WriteLine(product.Count);
            var result = new StoreDTO()
            {
                Id = storeId.Value,
                Products = product
            };
            return result;
        }
    }
}