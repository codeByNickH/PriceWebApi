namespace PriceWebApi.Models.DTO
{
    public class StoreLocationDTO
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int PostalCode { get; set; }
        public int ProductCount { get; set; }
        public ICollection<StoreDTO> Stores { get; set; }
    }    
}