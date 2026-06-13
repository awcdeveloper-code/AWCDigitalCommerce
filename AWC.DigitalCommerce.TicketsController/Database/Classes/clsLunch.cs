using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsLunch
    {
        public int ID { get; set; }
        public string LunchDate { get; set; }
        public string GUID { get; set; }
        public string EmployeeName { get; set; }
        public int Qty { get; set; }
        public int MealID { get; set; }
    }
}
