using System.ComponentModel.DataAnnotations;

namespace AWC.DigitalCommerce.API.Models.Domain
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Class { get; set; }
        public int Capacity { get; set; }
        public string? Zone { get; set; }
        public bool Available { get; set; }
    }
}
