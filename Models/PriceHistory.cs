using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceWebApi.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ComparePrice { get; set; }
        [MaxLength(10)]
        public string CompareUnit { get; set; }
        public bool WasDiscount { get; set; } = false;
        public DateTime RecordedAt { get; set; }
    }
}