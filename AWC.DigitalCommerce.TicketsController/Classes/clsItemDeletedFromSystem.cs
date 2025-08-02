using System;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsItemDeletedFromSystem
    {
        public int ID { get; set; }
        public string TicketDate { get; set; }
        public int ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int WhoDeleted { get; set; }
        public string WhoDeletedName { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedAtString { get; set; }
    }
}
