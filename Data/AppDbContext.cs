using Microsoft.EntityFrameworkCore;
using PriceWebApi.Models;


namespace PriceWebApi.Data.AppDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryList> CategoryLists { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreLocation> StoreLocations { get; set; }
    }
}