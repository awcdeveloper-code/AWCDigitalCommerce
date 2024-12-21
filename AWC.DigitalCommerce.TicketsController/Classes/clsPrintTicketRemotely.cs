using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsPrintTicketRemotely
    {
        public string GUID { get; set; }
        public string TicketForDataGrid { get; set; }

        public clsPrintTicketRemotely()
        {
            GUID = string.Empty;
            TicketForDataGrid = string.Empty;
        }
    }
}
