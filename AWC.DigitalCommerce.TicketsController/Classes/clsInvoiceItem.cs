using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsInvoiceItem
    {
        public string InvoiceGUID { get; set; }
        public int InvoiceItemID { get; set; }
        public int ItemType { get; set; }
        public int ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQty { get; set; }
    }
}
