using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsInternalOrders
    {
        public string OrderDate { get; set; }
        public string GUID { get; set; }
        public string OrderDescription { get; set; }
        public string WhoDidIt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class clsInternalOrdersDetail
    {
        public string GUID { get; set; }
        public string ItemDescription { get; set; }
        public int Qty { get; set; }
    }
}
