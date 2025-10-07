using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PriceWebApi.Data.AppDbContext;
using PriceWebApi.Models;
using PriceWebApi.Models.DTO;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Repositories
{
    public class StoreLocationRepository : IStoreLocationRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        public StoreLocationRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<List<StoreLocationDTO>> GetListOnFilterAndPageAsync(
            Expression<Func<StoreLocation, bool>> locationFilter = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var storesInArea = await context.StoreLocations
                .Where(locationFilter)
                .Select(l => new
                {
                    LocationId = l.Id,
                    City = l.City,
                    District = l.District,
                    Address = l.Address,
                    PostalCode = l.PostalCode,
                    Stores = l.Stores.Select(s => new
                    {
                        StoreId = s.Id,
                        StoreName = s.Name
                    }).ToList()
                }).ToListAsync();

            if (!storesInArea.Any())
                return new List<StoreLocationDTO>();

            var storeIds = storesInArea
                .SelectMany(l => l.Stores)
                .Select(s => s.StoreId)
                .ToList();

            var productTasks = storeIds.Select(async storeId =>
           {
               await using var context = await _dbContextFactory.CreateDbContextAsync();

               var products = await context.Products
                   .Include(p => p.Categories)
                       .ThenInclude(c => c.Category)
                   .Where(p => p.StoreId == storeId)
                   .OrderBy(p => p.WasDiscount ? 0 : 1)
                   .Skip(pageNumber * pageSize)
                   .Take(pageSize)
                   .AsNoTracking()
                   .ToListAsync();

               return new { StoreId = storeId, Products = products };
           }).ToList();

            var results = await Task.WhenAll(productTasks);

            var productsByStore = results
                .Where(r => r.Products.Any())
                .ToDictionary(r => r.StoreId, r => r.Products);

            var result = storesInArea.Select(location => new StoreLocationDTO
            {
                Id = location.LocationId,
                City = location.City,
                District = location.District,
                Address = location.Address,
                PostalCode = location.PostalCode,
                ProductCount = productsByStore.Values.Sum(p => p.Count),
                Stores = location.Stores.Select(store => new StoreDTO
                {
                    Id = store.StoreId,
                    Name = store.StoreName,
                    Products = productsByStore.ContainsKey(store.StoreId)
                        ? productsByStore[store.StoreId].Select(p => new ProductDTO
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
                        : new List<ProductDTO>()
                }).ToList()
            }).ToList();
            System.Console.WriteLine(productsByStore.Values.Sum(p => p.Count));
            return result;
        }

        public async Task<List<StoreLocationDTO>> GetListWithFilterAsync(
            Expression<Func<StoreLocation, bool>> locationFilter = null,
            Expression<Func<Product, bool>> productFilter = null,
            string sortBy = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var storesInArea = await context.StoreLocations
                .Where(locationFilter)
                .Select(l => new
                {
                    LocationId = l.Id,
                    City = l.City,
                    District = l.District,
                    Address = l.Address,
                    PostalCode = l.PostalCode,
                    Stores = l.Stores.Select(s => new
                    {
                        StoreId = s.Id,
                        StoreName = s.Name
                    }).ToList()
                }).ToListAsync();

            if (!storesInArea.Any())
                return new List<StoreLocationDTO>();

            var storeIds = storesInArea
                .SelectMany(l => l.Stores)
                .Select(s => s.StoreId)
                .ToList();

            var productTasks = storeIds.Select(async storeId =>
            {
                await using var context = await _dbContextFactory.CreateDbContextAsync();

                var query = context.Products
                    .Include(p => p.Categories)
                        .ThenInclude(c => c.Category)
                    .Where(p => p.StoreId == storeId)
                    .Where(productFilter); // Check if there is index on Category in DB

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

                var products = await query
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new { StoreId = storeId, Products = products };
            }).ToList();

            var results = await Task.WhenAll(productTasks);

            var productsByStore = results
                .Where(r => r.Products.Any())
                .ToDictionary(r => r.StoreId, r => r.Products);

            var result = storesInArea.Select(location => new StoreLocationDTO
            {
                Id = location.LocationId,
                City = location.City,
                District = location.District,
                Address = location.Address,
                PostalCode = location.PostalCode,
                ProductCount = productsByStore.Values.Sum(p => p.Count),
                Stores = location.Stores.Select(store => new StoreDTO
                {
                    Id = store.StoreId,
                    Name = store.StoreName,
                    Products = productsByStore.ContainsKey(store.StoreId)
                        ? productsByStore[store.StoreId].Select(p => new ProductDTO
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
                        : new List<ProductDTO>()
                }).ToList()
            }).ToList();
            System.Console.WriteLine(productsByStore.Values.Sum(p => p.Count));
            return result;
        }

        public async Task<List<StoreLocationDTO>> GetListWithSearchAsync(
            Expression<Func<StoreLocation, bool>> locationFilter = null,
            string searchString = null,
            string sortBy = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var storesInArea = await context.StoreLocations
                .Where(locationFilter)
                .Select(l => new
                {
                    LocationId = l.Id,
                    City = l.City,
                    District = l.District,
                    Address = l.Address,
                    PostalCode = l.PostalCode,
                    Stores = l.Stores.Select(s => new
                    {
                        StoreId = s.Id,
                        StoreName = s.Name
                    }).ToList()
                }).ToListAsync();

            if (!storesInArea.Any())
                return new List<StoreLocationDTO>();

            var storeIds = storesInArea
                .SelectMany(l => l.Stores)
                .Select(s => s.StoreId)
                .ToList();

            var productTasks = storeIds.Select(async storeId =>
            {
                var searchTerm = searchString.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                await using var context = await _dbContextFactory.CreateDbContextAsync();

                var query = context.Products
                    .Include(p => p.Categories)
                        .ThenInclude(c => c.Category)
                    .Where(p => p.StoreId == storeId);

                foreach (var term in searchTerm)
                {
                    query = query.Where(p => p.Name.Contains(term) || p.Brand.Contains(term) || p.CountryOfOrigin.Contains(term));
                }

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
                var products = await query
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new { StoreId = storeId, Products = products };
            }).ToList();

            var results = await Task.WhenAll(productTasks);

            var productsByStore = results
                .Where(r => r.Products.Any())
                .ToDictionary(r => r.StoreId, r => r.Products);

            var result = storesInArea.Select(location => new StoreLocationDTO
            {
                Id = location.LocationId,
                City = location.City,
                District = location.District,
                Address = location.Address,
                PostalCode = location.PostalCode,
                ProductCount = productsByStore.Values.Sum(p => p.Count),
                Stores = location.Stores.Select(store => new StoreDTO
                {
                    Id = store.StoreId,
                    Name = store.StoreName,
                    Products = productsByStore.ContainsKey(store.StoreId)
                        ? productsByStore[store.StoreId].Select(p => new ProductDTO
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
                        : new List<ProductDTO>()
                }).ToList()
            }).ToList();
            System.Console.WriteLine(productsByStore.Values.Sum(p => p.Count));
            return result;
        }
    }
}