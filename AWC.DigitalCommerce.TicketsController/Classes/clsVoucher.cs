using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsVoucher
    {
        public int ID { get; set; }
        public string BusinessDate { get; set; }
        public string IssueBy { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ExpireAt { get; set; }
    }
}
