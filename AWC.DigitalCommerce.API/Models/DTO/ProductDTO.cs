namespace AWC.DigitalCommerce.API.Models.DTO
{
    public class ProductDTO
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Type { get; set; }
        public bool Active { get; set; }
        public int Stock { get; set; }
        public int Minimum { get; set; }
    }
}
