namespace PriceWebApi.Models.DTO
{
    public class PriceHistoryDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal ComparePrice { get; set; }
        public string CompareUnit { get; set; }
        public bool WasDiscount { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}