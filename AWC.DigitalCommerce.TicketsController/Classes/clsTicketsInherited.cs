using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsTicketsInherited
    {
        public int ID { get; set; }
        public string TicketDate { get; set; }
        public int TicketID { get; set; }
        public string TicketGUID { get; set; }
        public string FromCustomer { get; set; }
        public string ToCustomer { get; set; }
        public string WhoMakeIt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
