using System.ComponentModel.DataAnnotations;

namespace AWC.DigitalCommerce.API.Models.Domain
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public int SeatId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Taxes { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Total { get; set; }
        public decimal Cash { get; set; }
        public decimal Card { get; set; }
        public decimal Transfer { get; set; }
        public decimal Voucher { get; set; }
        public int Status { get; set; }
        public string? SeatAKA { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PayedAt { get; set; }
    }
}
