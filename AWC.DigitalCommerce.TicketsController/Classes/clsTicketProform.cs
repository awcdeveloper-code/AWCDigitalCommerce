using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsTicketProform
    {
        public int ID { get; set; }
        public int TicketNumber { get; set; }
        public int TicketDetailID { get; set; }
        public string CustomerAKA { get; set; }
        public int ItemID { get; set; }
        public int Qty { get; set; }
    }
}
