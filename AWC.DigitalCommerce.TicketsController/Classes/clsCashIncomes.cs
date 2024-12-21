using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsCashIncomes
    {
        public int ID { get; set; }
        public string BusinessDate { get; set; }
        public string IncomeDescription { get; set; }
        public int IncomeAmount { get; set; }
        public string WhoDidIt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
