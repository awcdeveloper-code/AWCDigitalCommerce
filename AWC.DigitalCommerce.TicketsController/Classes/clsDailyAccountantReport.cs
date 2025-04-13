using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsDailyAccountantReport
    {
        public int ID { get; set; }
        public string BussinessDate { get; set; } = string.Empty;
        public int GrossSales { get; set; } = 0;
        public int NetSales { get; set; } = 0;
        public int Sales_Cash { get; set; } = 0;
        public int Sales_CreditCard { get; set; } = 0;
        public int Sales_Transfer { get; set; } = 0;
        public int Sales_Voucher { get; set; } = 0;
        public int Drawer_Cash { get; set; } = 0;
        public int Drawer_CreditCard { get; set; } = 0;
        public int Drawer_Transfer { get; set; } = 0;
        public int Drawer_Voucher { get; set; } = 0;
        public int DebitNotes { get; set; } = 0;
        public int CreditNotes { get; set; } = 0;
    }
}
