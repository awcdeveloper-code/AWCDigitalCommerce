using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsExpense
    {
        public int ID { get; set; }
        public string ExpenseDate { get; set; }
        public string ExpenseDescription { get; set; }
        public double ExpenseAmount { get; set; }
    }
}
