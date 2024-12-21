using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsTimeCard
    {
        public string BusinessDate { get; set; }
        public int UserPIN { get; set; }
        public int EventType { get; set; }
        public DateTime EventDatetime { get; set; }
    }
}
