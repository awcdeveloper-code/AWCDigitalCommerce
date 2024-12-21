using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsCustomerVIP
    {
        public int ID { get; set; }
        public  int Type { get; set; }
        public string CustomerID { get; set; }
        public bool Active { get; set; }
        public bool ApplyServiceFee { get; set; }
        public string LastPayment { get; set; }
        public bool CustomerFOC { get; set; }
        public int CreditLimit { get; set; }
        public string BirthDay { get; set; }
        public string MailAddress { get; set; }
        public string ImagePath { get; set; }
    }
}
