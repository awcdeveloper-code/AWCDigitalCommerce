using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsInvoice
    {
        public int ID { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int ProviderID { get; set; }
        public string ProviderName { get; set; }
        public double InvoiceAmount { get; set; }
        public string InvoiceGUID { get; set; }
    }
}
