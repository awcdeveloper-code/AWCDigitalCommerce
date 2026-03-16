using System.ComponentModel.DataAnnotations;

namespace AWC.DigitalCommerce.API.Models.DTO
{
    public class SeatDTO
    {
        public string? Name { get; set; }
        public int Type { get; set; }
        public int Capacity { get; set; }
        public string? Zone { get; set; }
        public bool Availale { get; set; }
    }
}
