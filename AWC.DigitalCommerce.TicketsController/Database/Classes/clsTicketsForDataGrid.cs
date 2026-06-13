using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsTicketsForDataGrid
    {
        public string TicketDate { get; set; }
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public int ServiceFee { get; set; }
        public int IVAFee { get; set; }
        public int TotalPrice { get; set; }
        public int Cash { get; set; }
        public int CreditCard { get; set; }
        public int Transfer { get; set; }
        public int Voucher { get; set; }
        public int PayMethod { get; set; }
        public string PayMethodAlpha { get; set; }
        public bool Status { get; set; }
        public int Splited { get; set; }
        public string StatusAlpha { get; set; }
        public string CustomerAKA { get; set; }
        public bool ApplyServiceFee { get; set; }
        public int Shift { get; set; }
        public string ImagePath { get; set; }
    }
}
