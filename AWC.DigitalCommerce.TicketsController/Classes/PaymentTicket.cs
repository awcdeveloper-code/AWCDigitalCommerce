namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class PaymentTicket
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public int TotalAmount { get; set; }
        public int Cash { get; set; }
        public int CreditCard { get; set; }
        public int Transfer { get; set; }
        public int Voucher { get; set; }
    }
}
