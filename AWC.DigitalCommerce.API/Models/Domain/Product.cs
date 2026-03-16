using System.ComponentModel.DataAnnotations;

namespace AWC.DigitalCommerce.API.Models.Domain
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
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
