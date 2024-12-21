using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsKitchenOrder
    {
        public int ID { get; set; }
        public int TicketNumber { get; set; }
        public string Detail  { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
