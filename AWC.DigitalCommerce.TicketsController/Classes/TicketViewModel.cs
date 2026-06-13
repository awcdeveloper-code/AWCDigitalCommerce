using System.Collections.Generic;

namespace AWC.DigitalCommerce.TicketsController
{
    public class TicketViewModel
    {
        public int ID { get; set; }
        public bool Active { get; set; }
        public string CustomerID { get; set; }
        public string GUID { get; set; }
        public List<TicketDetailSummary> Products { get; set; }
    }
}
