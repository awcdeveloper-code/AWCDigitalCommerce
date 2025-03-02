using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucDailyClosing_RestoBar : UserControl
    {
        private string lang = string.Empty;
        private string workDay = string.Empty;
        private List<clsTicketsForDataGrid> itemsList;
        private List<clsSmallPayment> smPaymentsList;
        private clsDailyClosing dcReport = new clsDailyClosing();
        private List<clsTicketsForDataGrid> itemsListSorted = new List<clsTicketsForDataGrid>();
        private int CashRegisterOpening = 0;
        private int CashOnDrawer = 0;
        private int CashWithdrawal = 0;
        public ucDailyClosing_RestoBar(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            SelectedDay.Text = DB.ConverTicketDate(Settings.Default.BusinessDate).Replace(".", "/");

            if (SMTP.CheckInternetConnection())
            {
                SendReportByEmail.IsChecked = true;
                SendReportByEmail.IsEnabled = true;
            }
            else
            {
                SendReportByEmail.IsChecked = false;
                SendReportByEmail.IsEnabled = false;
                Helper.ShowToastNotification("ATENCIÓN: Sin acceso a Internet");
            }

            PrintSummaryWithDetail.IsChecked = Settings.Default.PrintSummaryWithDetail;
            this.GroupBoxDataGrid.Header += $" - TURNO {Settings.Default.Shift}";
            txtShift.Text = Settings.Default.Shift.ToString();
        }
        private void SelectedDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            workDay = SelectedDay.SelectedDate.ToString();

            if (workDay.Length == 0) return;

            string year = workDay.Split('/')[2].Substring(0, 4);
            string month = workDay.Split('/')[1].PadLeft(2, '0');
            string day = workDay.Split('/')[0].PadLeft(2, '0');

            workDay = year + month + day;

            txtShift.Text = string.Empty;
            txtShift.Focus();
        }
        private void btn_DailyClosePrint(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Ticket Controller", $"CONFIRMACIÓN: DESEA HACER EL CIERRE DEL TURNO {Settings.Default.Shift} (SI/NO)?",
                MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, null) == MessageBoxResult.No)
            {
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            DB.InsertDailyClosingSummary(dcReport);

            if (PrintSummaryWithDetail.IsChecked == true)
            {
                Helper.PrintTicket(workDay, itemsList, 0);
            }
            else if (Settings.Default.PrintDailyClosingReport)
            {
                Helper.PrintTicket(workDay, dcReport);
            }

            if (Settings.Default.ReportsRepository)
                ReportsRepository.DailyClosing(workDay, itemsList);

            if (SendReportByEmail.IsChecked == true && Settings.Default.eMailDistributionList.Length > 0)
            {
                SMTP.SendDailyReport(dcReport, workDay, itemsList);
            }

            DB.AssignShiftToDailyClosing(Settings.Default.Shift, workDay);

            SelectedDay.SelectedDate = null;
            DailyClosePrint.IsEnabled = false;
            Mouse.OverrideCursor = null;

            if (Settings.Default.Shift == Settings.Default.ShiftForQuery)
            {
                Helper.ShowToastNotification($"Cierre del Turno {Settings.Default.Shift} concluido");
                Settings.Default.Shift++;
                Settings.Default.Save();
            }
            else
            {
                Helper.ShowToastNotification($"Reproceso de cierre del Turno {Settings.Default.Shift} concluido");
            }

            Mouse.OverrideCursor = null;

            if (Settings.Default.CashOnDrawer)
            {
                this.Opacity = 0.5;
                wpfCashOnDrawer cashOnDrawer = new wpfCashOnDrawer(CashOnDrawer);
                cashOnDrawer.ShowDialog();
                this.Opacity = 1;
                dcReport.CashWithdrawal = cashOnDrawer.CashWithdrawal;
                DB.UpdateCashOnHandAtTheBeginning(CashOnDrawer - cashOnDrawer.CashWithdrawal);
            }
        }
        private void PrintSummaryWithDetail_Checked(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintSummaryWithDetail == false)
                Settings.Default.PrintSummaryWithDetail = true;
            else
                Settings.Default.PrintSummaryWithDetail = false;

            Settings.Default.Save();
        }
        private void txtShift_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtShift.Text.Length == 0)
            {
                lbl_IncomeCash.Content = "0";
                lbl_OutstandingAmount.Content = "0";
                lbl_Cash.Content = "0";
                lbl_CreditCard.Content = "0";
                lbl_Transfer.Content = "0";
                lbl_Voucher.Content = "0";
                lbl_TotalSale.Content = "0";
                lbl_NetSale.Content = "0";
                lbl_ServiceFee.Content = "0";
                lbl_Expenses.Content = "0";
                lbl_OldTicketsPay.Content = "0";

                List<clsTicketsForDataGrid> itemsCleaner = new List<clsTicketsForDataGrid>();
                TodayTickets.ItemsSource = itemsCleaner;
                return;
            }

            if (Convert.ToInt32(txtShift.Text) > Settings.Default.Shift)
            {
                wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: NÚMERO DE TURNO SOLICITADO NO PUEDE SER MAYOR AL TURNO ACTIVO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                txtShift.Text = Settings.Default.Shift.ToString();
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            Settings.Default.ShiftForQuery = Convert.ToInt32(txtShift.Text);
            Settings.Default.Save();

            clsTicket ticketSummary = DB.GetTicketsSummary(workDay);
            clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(workDay);
            List<clsExpense> expensesList = DB.GetExpenses(workDay);
            List<clsCashIncomes> incomeCashList = DB.GetIncomeCash(workDay, workDay);

            ticketSummary.Cash += smlPay.Cash;
            ticketSummary.CreditCard += smlPay.CreditCard;
            ticketSummary.Transfer += smlPay.Transfer;
            ticketSummary.Voucher += smlPay.Voucher;

            int oldTicketsCancelled = DB.GetOldTicketsCancelled(workDay);

            int totalPrice = ticketSummary.Cash +
                             ticketSummary.CreditCard +
                             ticketSummary.Transfer +
                             ticketSummary.Voucher +
                             ticketSummary.Payments -
                             oldTicketsCancelled;

            int netPrice = ticketSummary.Cash +
                           ticketSummary.CreditCard +
                           ticketSummary.Transfer +
                           ticketSummary.Voucher;

            int totExp = (int)expensesList.Sum(x => x.ExpenseAmount);

            if (workDay != Settings.Default.BusinessDate)
            {
                CashRegisterOpening = DB.GetInitialCashFromDailyClosingSummary(workDay);
                lbl_InitialCash.Content = CashRegisterOpening.ToString("N0");
            }
            else
            {
                CashRegisterOpening = DB.GetCashOnHandAtTheBeginning();
                lbl_InitialCash.Content = CashRegisterOpening.ToString("N0");
            }

            int incomeCash = incomeCashList.Sum(x => x.IncomeAmount);
            lbl_IncomeCash.Content = incomeCash.ToString("N0");
            lbl_OutstandingAmount.Content = ticketSummary.Payments.ToString("N0");
            lbl_Cash.Content = (CashRegisterOpening + incomeCash + ticketSummary.Cash - totExp).ToString("N0");
            lbl_CreditCard.Content = ticketSummary.CreditCard.ToString("N0");
            lbl_Transfer.Content = ticketSummary.Transfer.ToString("N0");
            lbl_Voucher.Content = ticketSummary.Voucher.ToString("N0");
            lbl_TotalSale.Content = totalPrice.ToString("N0");
            lbl_NetSale.Content = netPrice.ToString("N0");
            lbl_ServiceFee.Content = ticketSummary.ServiceFee.ToString("N0");
            lbl_Expenses.Content = expensesList.Sum(x => x.ExpenseAmount).ToString("N0");
            lbl_OldTicketsPay.Content = (oldTicketsCancelled + smlPay.Cash + smlPay.CreditCard + smlPay.Transfer + smlPay.Voucher).ToString("N0");

            CashOnDrawer = CashRegisterOpening + incomeCash + ticketSummary.Cash - totExp;

            dcReport = new clsDailyClosing();
            dcReport.BusinessDate = workDay;
            dcReport.Shift = Settings.Default.Shift;
            dcReport.InitialCash = CashRegisterOpening;
            dcReport.IncomeCash = incomeCash;
            dcReport.AccountsReceivable = ticketSummary.Payments;
            dcReport.Cash = ticketSummary.Cash;
            dcReport.CreditCard = ticketSummary.CreditCard;
            dcReport.Transfer = ticketSummary.Transfer;
            dcReport.Voucher = ticketSummary.Voucher;
            dcReport.GrossSale = totalPrice;
            dcReport.NetSale = netPrice;
            dcReport.ServiceFee = ticketSummary.ServiceFee;
            dcReport.Expenses = expensesList.Sum(x => x.ExpenseAmount);
            dcReport.TotalCashInDrawer = CashRegisterOpening + dcReport.NetSale;
            dcReport.OldTicketsPay = oldTicketsCancelled + smlPay.Cash + smlPay.CreditCard + smlPay.Transfer + smlPay.Voucher;
            dcReport.ExpensesList = expensesList;
            dcReport.VouchersList = DB.GetVouchers(Settings.Default.BusinessDate);

            itemsList = DB.DataBinding_tbl_DailyClose(workDay);

            smPaymentsList = DB.GetSmallPayments(workDay);

            foreach (clsSmallPayment smlPayment in smPaymentsList)
            {
                clsTicketsForDataGrid item = new clsTicketsForDataGrid();

                item.ID = smlPayment.TicketID;
                item.CustomerID = "ABONO A LA CUENTA";
                item.TotalPrice = smlPayment.PaymentAmount;
                item.PayMethod = 1;
                item.PayMethodAlpha = "CANC";
                item.Status = false;
                item.StatusAlpha = "CANC";

                itemsList.Add(item);
            }

            itemsListSorted = Helper.SortTicketsForDataGrid(itemsList);

            TodayTickets.ItemsSource = itemsListSorted;

            Mouse.OverrideCursor = null;

            DailyClosePrint.IsEnabled = true;
            DailyClosePrint.Focus();
        }
    }
}
