using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsTicket
    {
        public int ID { get; set; }
        public string TicketDate { get; set; }
        public string GUID { get; set; }
        public int CustID { get; set; }
        public int TotalPrice { get; set; }
        public int ServiceFee { get; set; }
        public int IVAFee { get; set; }
        public int Payments { get; set; }
        public int Cash { get; set; }
        public int CreditCard { get; set; }
        public int Transfer { get; set; }
        public int Voucher { get; set; }
        public int CashLoan { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime CloseAt { get; set; }
        public int PayMethod { get; set; }
        public string PayMethodAlpha { get; set; }
        public bool Status { get; set; }
        public string StatusAlpha { get; set; }
        public int WhoOpened { get; set; }
        public int WhoClosed { get; set; }
        public bool Splited { get; set; }
        public string CustomerAKA { get; set; }
        public bool ApplyServiceFee { get; set; }
        public string AbortReason { get; set; }
        public int Shift { get; set; }
    }

    public class clsTicketModified
    {
        public int ID { get; set; }
        public string oriTicketDate { get; set; }
        public int oriCustID { get; set; }
        public int oriTotalPrice { get; set; }
        public int oriServiceFee { get; set; }
        public int oriPayments { get; set; }
        public int oriCash { get; set; }
        public int oriCreditCard { get; set; }
        public int oriTransfer { get; set; }
        public string oriCreateAt { get; set; }
        public string modTicketDate { get; set; }
        public int modCustID { get; set; }
        public int modTotalPrice { get; set; }
        public int modServiceFee { get; set; }
        public int modPayments { get; set; }
        public int modCash { get; set; }
        public int modCreditCard { get; set; }
        public int modTransfer { get; set; }
        public DateTime modCreateAt { get; set; }
    }
}
