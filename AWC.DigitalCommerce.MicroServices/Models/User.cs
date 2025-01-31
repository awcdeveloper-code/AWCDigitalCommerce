using System.ComponentModel.DataAnnotations;

namespace AWC.DigitalCommerce.MicroServices.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public int PIN { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public int UserStatus { get; set; }
        public string? UserSecurityProfile { get; set; }
    }
}
