using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using PriceWebApi.Helpers;
using PriceWebApi.Models;
using PriceWebApi.Models.ApiResponse;
using PriceWebApi.Models.DTO;
using PriceWebApi.Repositories.IRepositories;

namespace PriceWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IStoreLocationRepository _locationRepository;
        private readonly IRepository<Product> _productRepository;
        public ProductController(IRepository<Product> productRepository, IStoreLocationRepository locationRepository)
        {
            _productRepository = productRepository;
            _locationRepository = locationRepository;
        }
        [HttpGet("GetProducts")]
        public async Task<ActionResult<ApiResponse>> GetProducts(
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City parameter is required.");
            }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }

            Expression<Func<StoreLocation, bool>> filter = CreateLocationFilter(city, district);

            if (filter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListOnFilterAndPageAsync(filter, page));
        }
        [HttpGet("GetSingleProduct")]
        public async Task<ActionResult<ApiResponse>> GetSingleProduct(int productId)
        {
            return ResponseExtension.CreateApiResponse(await _productRepository.GetOnFilterAsync(x => x.Id == productId));
        }
        [HttpGet("GetProductsByFilter")]
        public async Task<ActionResult<ApiResponse>> GetProductsByFilter(
            [FromQuery] string searchTerm = null,
            [FromQuery] string filterType = "textSearch",
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required.");
            }
            if (string.IsNullOrEmpty(filterType))
            {
                return BadRequest("Filter type is required. Valid values: textSearch, category, country");
            }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }

            Expression<Func<StoreLocation, bool>> filter = CreateLocationFilter(city, district);
            Expression<Func<Product, bool>> productFilter = CreateProductFilter(filterType.ToLower(), searchTerm);

            if (filter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }
            if (productFilter == null)
            {
                return BadRequest("Invalid filter type. Valid values: textSearch, category, country");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListWithFilterAsync(filter, productFilter, page));
        }
        [HttpGet("GetProductsByCategory")]
        public async Task<ActionResult<ApiResponse>> GetProductsByCategory(
            [FromQuery] string category = null,
            [FromQuery] string city = null,
            [FromQuery] string district = null,
            [FromQuery] int page = 0)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("Category parameter is required.");
            }
            if (page < 0)
            {
                return BadRequest("Page number parameter is required and must be 0 or greater.");
            }

            Expression<Func<StoreLocation, bool>> filter = CreateLocationFilter(city, district);
            Expression<Func<Product, bool>> productFilter = CreateProductFilter("category", category);

            if (filter == null)
            {
                return BadRequest("Invalid location, city must be provided.");
            }

            return ResponseExtension.CreateApiResponse(await _locationRepository.GetListWithFilterAsync(filter, productFilter, page));
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
            return filterType switch
            {
                "textsearch" => p => p.Name.Contains(searchTerm) || p.Brand.Contains(searchTerm),
                "country" => p => p.CountryOfOrigin.Contains(searchTerm),
                "category" => p => p.Categories.Any(c => c.Category.Name == searchTerm),
                _ => null
            };
        }
    }
}