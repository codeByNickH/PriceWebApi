using System.ComponentModel.DataAnnotations.Schema;

namespace PriceWebApi.Models
{
    public class CategoryList
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        // public Product Product { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int StoreId { get; set; }
    }
}