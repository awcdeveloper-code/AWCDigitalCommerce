using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsOpenDrawerRequest
    {
        public int ID { get; set; }
        public string BusinessDate { get; set; }
        public int WhoOpen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
