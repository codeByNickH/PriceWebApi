# PriceWebApi

A high-performance ASP.NET Core Web API for comparing product prices across multiple store locations. Built with Entity Framework Core and optimized for efficient querying of large product catalogs.

> **Note:** This is an active commercial project - a price comparison platform for grocery stores in Sweden. The code is publicly available to demonstrate technical abilities to potential employers, but remains proprietary software.

## Overview

PriceWebApi enables users to search and compare prices for products across different stores in specific geographic locations. The API supports advanced filtering, sorting, and pagination to help users find the best deals.

## Features

- **Location-based product search** - Find products available at stores in specific cities and districts
- **Multi-store price comparison** - View prices from multiple stores side-by-side
- **Advanced product filtering** - Filter by text search, category, or country of origin
- **Flexible sorting options** - Sort by price (low/high), discount percentage, or compare price
- **Pagination support** - Efficient data loading with per-store pagination
- **Price history tracking** - Track price changes over time
- **Discount highlighting** - Identify products currently on sale

## Tech Stack

- **Framework**: .NET 8.0
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: Microsoft Identity Web with Azure AD (JWT Bearer)
- **API Documentation**: Swagger/OpenAPI
- **Architecture Pattern**: Repository Pattern with async/await

## Project Structure

```
PriceWebApi/
├── Controllers/
│   ├── ProductController.cs       # Main API endpoints
│   ├── ShoppingListController.cs  # Shopping list features (planned)
│   └── UserController.cs          # User management (planned)
├── Data/
│   └── AppDbContext.cs            # EF Core database context
├── Models/
│   ├── Product.cs                 # Product entity
│   ├── Store.cs                   # Store entity
│   ├── StoreLocation.cs           # Store location entity
│   ├── Category.cs                # Category entity
│   ├── CategoryList.cs            # Product-Category junction table
│   ├── PriceHistory.cs            # Price tracking entity
│   └── DTO/                       # Data Transfer Objects
├── Repositories/
│   ├── IRepositories/             # Repository interfaces
│   ├── ProductRepository.cs
│   ├── StoreLocationRepository.cs
│   └── StoreRepository.cs
├── Helpers/
│   └── ResponseExtension.cs       # API response formatting
└── Services/
    └── OptimizedProductService.cs # Caching layer (planned)
```

## Database Schema

### Core Entities

**StoreLocation**
- Represents physical store locations with address information
- Links to multiple Store entities

**Store**
- Individual stores (e.g., "ICA Maxi", "Coop Extra")
- Belongs to a StoreLocation
- Contains multiple Products

**Product**
- Product information including prices, discounts, and metadata
- Belongs to a Store
- Has price history tracking

**Category & CategoryList**
- Product categorization with many-to-many relationship
- Supports multiple categories per product

**PriceHistory**
- Historical price tracking for trend analysis
- Records price changes, discounts, and timestamps

## API Endpoints

### Get Products (First Load)
```http
GET /api/Product/GetProducts?city={city}&district={district}&page={page}
```
Returns paginated products from stores in the specified location.

**Parameters:**
- `city` (required): City name
- `district` (optional): District/neighborhood
- `page` (default: 0): Page number for pagination

### Search Products
```http
GET /api/Product/GetProductsBySearch?searchTerm={term}&filterType={type}&sortBy={sort}&city={city}&page={page}
```
Search for products with advanced filtering and sorting.

**Parameters:**
- `searchTerm` (required): Search query
- `filterType` (default: "textsearch"): Filter type - `textsearch`, `category`, `country`
- `sortBy` (default: "discountLowest"): Sort option
- `city` (required): City name
- `district` (optional): District name
- `page` (default: 0): Page number

**Sort Options:**
- `lowprice` - Lowest price first
- `highprice` - Highest price first
- `lowcompareprice` - Lowest unit price first
- `highcompareprice` - Highest unit price first
- `discountcompareprice` - Discounts first, then by unit price
- `discountpercentage` - Highest discount percentage first

### Get Products by Category
```http
GET /api/Product/GetProductsByCategory?category={category}&sortBy={sort}&city={city}&page={page}
```
Filter products by category with sorting options.

### Get Single Product
```http
GET /api/Product/GetSingleProduct?productId={id}
```
Retrieve detailed information for a specific product including price history.

## Response Format

All endpoints return a standardized API response:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "id": 1,
    "city": "Stockholm",
    "district": "Södermalm",
    "address": "Götgatan 100",
    "postalCode": 11862,
    "productCount": 30,
    "stores": [
      {
        "id": 1,
        "name": "ICA Maxi",
        "products": [
          {
            "id": 101,
            "name": "Milk 3%",
            "brand": "Arla",
            "currentPrice": 15.90,
            "originalPrice": 18.90,
            "discountPercentage": 15.87,
            "wasDiscount": true,
            "categories": [
              {"id": 1, "name": "Dairy"}
            ]
          }
        ]
      }
    ]
  },
  "errorMessage": null
}
```

## Performance Optimizations

The API implements several performance optimizations:

1. **DbContext Factory Pattern** - Creates separate database contexts for parallel queries
2. **Parallel Store Queries** - Fetches products from multiple stores simultaneously
3. **Efficient Pagination** - Per-store pagination with Skip/Take at database level
4. **Strategic Indexing** - Database indexes on frequently queried columns (recommended)
5. **AsNoTracking** - Read-only queries for better performance
6. **Projection to DTOs** - Reduces data transfer by selecting only needed fields

### Database Indexes

For optimal performance:

```sql
CREATE INDEX IX_Products_StoreId ON Products(StoreId);
CREATE INDEX IX_Products_CurrentPrice ON Products(CurrentPrice);
CREATE INDEX IX_Products_WasDiscount ON Products(WasDiscount);
CREATE INDEX IX_Products_CurrentComparePrice ON Products(CurrentComparePrice);
CREATE INDEX IX_Products_WasDiscount_CurrentPrice ON Products(WasDiscount, CurrentPrice);
CREATE INDEX IX_CategoryLists_ProductId ON CategoryLists(ProductId);
CREATE INDEX IX_CategoryLists_CategoryId ON CategoryLists(CategoryId);
```

## Technical Highlights for Employers

This project demonstrates:

- **Clean Architecture** - Repository pattern with dependency injection
- **Performance Optimization** - DbContext factory pattern for parallel queries, strategic use of AsNoTracking, efficient pagination
- **Async/Await Mastery** - All database operations use async patterns correctly
- **LINQ Expertise** - Complex queries with Include/ThenInclude, dynamic filtering and sorting
- **API Design** - RESTful endpoints with standardized response format
- **Entity Framework Core** - Advanced usage including expression trees, projections to DTOs
- **Problem Solving** - Solved N+1 query problem, implemented per-store pagination with parallel execution

## Development Roadmap

### Planned Features
- [ ] Shopping list functionality
- [ ] User accounts and preferences
- [ ] Price alerts and notifications
- [ ] Store favorite products
- [ ] Advanced caching with Redis
- [ ] Rate limiting
- [ ] API versioning
- [ ] Product image optimization
- [ ] Elasticsearch integration for full-text search

### Known Limitations
- ProductCount in StoreLocationDTO currently sums all stores (needs fix)
- No authentication enforcement (Azure AD configured but not required)
- Limited error handling for malformed requests
- No API rate limiting

## License

© 2025. All rights reserved.

This is proprietary software developed for a commercial price comparison service. The source code is publicly visible for portfolio and demonstration purposes only. Unauthorized copying, distribution, modification, or commercial use of this software is strictly prohibited.

## Acknowledgments

Built with:
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Microsoft Identity Web](https://github.com/AzureAD/microsoft-identity-web)
