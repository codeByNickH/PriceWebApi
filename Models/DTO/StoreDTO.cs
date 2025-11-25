namespace PriceWebApi.Models.DTO
{
    public class StoreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDTO> Products { get; set; }
    }
}