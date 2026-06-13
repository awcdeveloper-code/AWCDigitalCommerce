using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsProvider
    {
        public int ID { get; set; }
        public string ProviderName { get; set; }
        public string BusinessAddress { get; set; }
        public string eMailAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PhoneNumber { get; set; }
        public string CellularNumber { get; set; }
        public string Remarks { get; set; }
    }
}
