using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsSmallPayment
    {
        public int ID { get; set; }
        public string PaymentDate { get; set; }
        public string RandomRef { get; set; }
        public int CustomerID { get; set; }
        public int TicketID { get; set; }
        public int CurTotalPrice { get; set; }
        public int PaymentAmount { get; set; }
        public int Cash { get; set; }
        public int CreditCard { get; set; }
        public int Transfer { get; set; }
        public int Voucher { get; set; }
        public int NewTotalPrice { get; set; }
        public int WhoClosed { get; set; }
    }
}
