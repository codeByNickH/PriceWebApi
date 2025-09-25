using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceWebApi.Models
{
    public class Store
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        public int StoreLocationId { get; set; }
        [ForeignKey("StoreLocationId")]
        public StoreLocation StoreLocation { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}