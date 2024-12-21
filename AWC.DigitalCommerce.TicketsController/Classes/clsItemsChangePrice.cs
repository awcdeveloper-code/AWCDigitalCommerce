using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsItemsChangePrice
    {
        public int ID { get; set; }
        public string BusinessDate {get; set; }
        public int ItemID { get; set; }
        public int PreviousPrice { get; set; }
        public int CurrentPrice { get; set; }
        public string WhoDidit { get; set; }
        public DateTime MadeItAt { get; set; }
    }
}
