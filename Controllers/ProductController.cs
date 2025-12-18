using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PriceWebApi.Helpers;
using PriceWebApi.Models;
using PriceWebApi.Models.ApiResponse;
using PriceWebApi.Models.DTO;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("ProductApi")]
    public class ProductController : ControllerBase
    {
        private readonly IStoreLocationRepository _locationRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IRepository<Product> _productRepository;
        public ProductController(IRepository<Product> productRepository, IStoreLocationRepository locationRepository, IStoreRepository storeRepository)
        {
            _productRepository = productRepository;
            _locationRepository = locationRepository;
            _storeRepository = storeRepository;
        }
        [HttpGet("Products")] // First page load + pagination
        public async Task<ActionResult<ApiResponse>> GetProducts(
            [FromQuery] string sortBy = "default",
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City parameter is required.");
            }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            Expression<Func<StoreLocation, bool>> locationFilter = CreateLocationFilter(city, district);

            if (locationFilter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListOnFilterAndPageAsync(locationFilter, sortBy, page, pageSize));
        }
        [HttpGet("ID")] // Click on product to get more info
        public async Task<ActionResult<ApiResponse>> GetSingleProduct(
            [FromQuery] int productId = 0)
        {
            return ResponseExtension.CreateApiResponse(await _productRepository.GetOnFilterAsync(x => x.Id == productId));
        }
        [HttpGet("Search")] // Search for product
        public async Task<ActionResult<ApiResponse>> GetProductsBySearch(
            [FromQuery] string searchTerm = null,
            // [FromQuery] string filterType = "textsearch",
            [FromQuery] string sortBy = "default",
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required.");
            }
            // var validFilterTypes = new[] { "textsearch", "category", "country" };
            // if (!validFilterTypes.Contains(filterType.ToLowerInvariant()))
            // {
            //     return BadRequest($"Filter type is required. Valid values: {string.Join(", ", validFilterTypes)}");
            // }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            Expression<Func<StoreLocation, bool>> locationFilter = CreateLocationFilter(city, district);

            if (locationFilter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListWithSearchAsync(locationFilter, searchTerm, sortBy, page, pageSize));
        }
        [HttpGet("Categories/")] // Products in a category
        public async Task<ActionResult<ApiResponse>> GetProductsByCategory(
            [FromQuery] string category = null, // Change to Id?
            [FromQuery] string sortBy = "default",
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("Category parameter is required.");
            }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            Expression<Func<StoreLocation, bool>> locationFilter = CreateLocationFilter(city, district);
            Expression<Func<Product, bool>> productFilter = CreateProductFilter("category", category);

            if (locationFilter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListWithFilterAsync(locationFilter, productFilter, sortBy, page, pageSize));
        }
        [HttpGet("Store/")]
        public async Task<ActionResult<ApiResponse>> GetProductsByStore(
            [FromQuery] string category = null, // Change to Id?
            [FromQuery] string sortBy = "default",
            [FromQuery] int? storeId = 0,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("Category parameter is required.");
            }
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            Expression<Func<Product, bool>> productFilter = CreateProductFilter("category", category);

            if (!storeId.HasValue)
            {
                return BadRequest("Store Id must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _storeRepository.GetSingleStoreListWithFilterAsync(storeId, productFilter, sortBy, page, pageSize));
        }
        [HttpGet("Store/Search")]
        public async Task<ActionResult<ApiResponse>> GetProductsByStoreSearch(
            [FromQuery] string searchTerm = null,
            [FromQuery] string sortBy = "default",
            [FromQuery] int? storeId = 0,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required.");
            }
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            if (!storeId.HasValue)
            {
                return BadRequest("Store Id must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _storeRepository.GetSingleStoreListWithSearchAsync(storeId, searchTerm, sortBy, page, pageSize));
        }
        [HttpGet("Store/Products")]
        public async Task<ActionResult<ApiResponse>> GetProductsByStore(
            [FromQuery] string sortBy = "default",
            [FromQuery] int? storeId = 0,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            if (pageSize < 10 || pageSize > 30)
            {
                return BadRequest("Page size must be between 10 and 30");
            }

            if (!storeId.HasValue)
            {
                return BadRequest("Store Id must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _storeRepository.GetSingleStoreListFirstPageAsync(storeId, sortBy, page, pageSize));
        }
        private Expression<Func<StoreLocation, bool>> CreateLocationFilter(string city, string district)
        {
            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(district))
            {
                return x => x.City == city && x.District == district;
            }
            if (!string.IsNullOrEmpty(city))
            {
                return x => x.City == city;
            }
            if (!string.IsNullOrEmpty(district))
            {
                return x => x.District == district;
            }
            return null;
        }
        private Expression<Func<Product, bool>> CreateProductFilter(string filterType, string searchTerm)

        {
            return filterType?.ToLowerInvariant() switch
            {
                "textsearch" => p =>
                    (p.Name != null && p.Name.Contains(searchTerm)) ||
                    (p.Brand != null && p.Brand.Contains(searchTerm)),
                "country" => p =>
                    p.CountryOfOrigin != null &&
                    p.CountryOfOrigin.Contains(searchTerm),
                "category" => p =>
                    p.Categories.Any(c => c.Category != null &&
                    c.Category.Name == searchTerm),
                _ => null
            };
        }
    }
}