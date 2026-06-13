using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsPayMethodChange
    {
        public int ID { get; set; }
        public string TicketDate { get; set; }
        public int TicketID { get; set; }
        public int OrigCash { get; set; }
        public int OrigCreditCard { get; set; }
        public int OrigTransfer { get; set; }
        public int CurrCash { get; set; }
        public int CurrCreditCard { get; set; }
        public int CurrTransfer { get; set; }
        public string WhoDidIt { get; set; }
        public DateTime MadeItAt { get; set; }
    }
}
