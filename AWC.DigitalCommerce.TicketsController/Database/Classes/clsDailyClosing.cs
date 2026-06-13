using AWC.DigitalCommerce.TicketsController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsDailyClosing
    {
        public string BusinessDate { get; set; }
        public int Shift { get; set; }
        public int InitialCash { get; set; }
        public int IncomeCash { get; set; }
        public int AccountsReceivable { get; set; }
        public int Cash { get; set; }
        public int CreditCard { get; set; }
        public int Transfer { get; set; }
        public int Voucher { get; set; }
        public int CashByOperator { get; set; }
        public int CreditCardByOperator { get; set; }
        public int TransferByOperator { get; set; }
        public int VoucherByOperator { get; set; }
        public int GrossSale { get; set; }
        public int NetSale { get; set; }
        public int ServiceFee { get; set; }
        public double Expenses { get; set; }
        public int TotalCashInDrawer { get; set; }
        public int OldTicketsPay { get; set; }
        public bool DailyClosingMatch { get; set; }
        public string WhoDidIt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<clsCashIncomes> CashIncomeList { get; set; }
        public List<clsExpense> ExpensesList { get; set; }
        public int CashWithdrawal { get; set; }
        public int Vouchers { get; set; }
        public List<clsVoucher> VouchersList { get; set; }
    }
}
