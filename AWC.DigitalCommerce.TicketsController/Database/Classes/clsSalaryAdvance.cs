using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsSalaryAdvance
    {
        public int ID { get; set; }
        public string BusinessDate { get; set; }
        public string Requester { get; set; }
        public int Approver { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
