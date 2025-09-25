namespace PriceWebApi.Models.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string CountryOfOrigin { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal CurrentComparePrice { get; set; }
        public decimal? ComparePrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? Size { get; set; }
        public string Unit { get; set; }
        public string MaxQuantity { get; set; }
        public string MinQuantity { get; set; }
        public bool MemberDiscount { get; set; }
        public bool WasDiscount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<CategoryDTO> Categories { get; set; }
    }
}