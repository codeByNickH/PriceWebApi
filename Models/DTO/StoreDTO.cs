namespace PriceWebApi.Models.DTO
{
    public class StoreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public string City { get; set; }
        // public string District { get; set; }
        // public string Address { get; set; }
        public ICollection<ProductDTO> Products { get; set; }
    }
}