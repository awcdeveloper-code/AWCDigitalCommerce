using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucExpensesReport.xaml
    /// </summary>
    public partial class ucExpensesReport : System.Windows.Controls.UserControl
    {
        // MESSAGES
        private string lang = string.Empty;
        private string workDay = string.Empty;
        public string strExpenseAdded = string.Empty;
        public string strLunchAdded = string.Empty;

        public ucExpensesReport(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            //Traductor.ApplyTranslation(this, lang);

            DateTime selectedDateTime = new DateTime(Convert.ToInt32(Settings.Default.BusinessDate.Substring(0,4)),
                                                     Convert.ToInt32(Settings.Default.BusinessDate.Substring(4, 2)),
                                                     Convert.ToInt32(Settings.Default.BusinessDate.Substring(6, 2)),0, 0, 0);

            datePicker.SelectedDate = selectedDateTime;
            lunchDatePicker.SelectedDate = selectedDateTime;
            cbox_Meals.DataContext = DB.DataBinding_tbl_Items(3);   // read Meals
        }

        #region UTILTIES
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CleanAll(string groupName)
        {
            switch(groupName)
            {
                case "Expense":
                    datePicker.SelectedDate = DateTime.Today;
                    txtExpenseDescription.Text = string.Empty;
                    txtExpenseAmount.Text = string.Empty;
                    Add.IsEnabled = false;
                    break;
                case "Lunch":
                    lunchDatePicker.SelectedDate = DateTime.Today;
                    txtEmployeeName.Text = string.Empty;
                    cbox_Meals.SelectedIndex = -1;
                    txtQty.Text = string.Empty;
                    AddLunch.IsEnabled = false;
                    break;
                case "Advance":
                    AdvDatePicker.SelectedDate = DateTime.Today;
                    txtAdvRequester.Text = string.Empty;
                    txtAdvAmount.Text = string.Empty;
                    AddAdv.IsEnabled = false;
                    break;
            }
        }
        #endregion

        #region GENERAL EXPENSES
        private void txtExpenseAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtExpenseAmount.Text = numKey.numKeyed;
            Add.IsEnabled = true;
        }

        private void btn_AddExpense(object sender, RoutedEventArgs e)
        {
            if (txtExpenseDescription.Text.Length > 0 && txtExpenseAmount.Text.Length >0)
            {
                string dt = datePicker.SelectedDate.ToString();

                if (dt.Length == 0) return;

                string year = dt.Split('/')[2].Substring(0, 4);
                string month = dt.Split('/')[1].PadLeft(2, '0');
                string day = dt.Split('/')[0].PadLeft(2, '0');

                dt = year + month + day;

                if (DB.InsertNewExpense(dt, txtExpenseDescription.Text.ToUpper(), Convert.ToDouble(txtExpenseAmount.Text)))
                    Helper.ShowToastNotification("Gasto Interno Contabilizado");
            }
            CleanAll("Expense");
        }
        #endregion

        #region EMPOYEES LUNCH
        private void lunchDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            workDay = lunchDatePicker.SelectedDate.ToString();

            if (workDay.Length == 0) return;

            string year = workDay.Split('/')[2].Substring(0, 4);
            string month = workDay.Split('/')[1].PadLeft(2, '0');
            string day = workDay.Split('/')[0].PadLeft(2, '0');

            workDay = year + month + day;
        }
        
        private void cbox_Meals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_Meals.SelectedIndex == -1) return;

            AddLunch.IsEnabled = true;
        }

        private void txtQty_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQty.Text = numKey.numKeyed;
        }

        private void btn_AddLunch(object sender, RoutedEventArgs e)
        {
            if (lunchDatePicker.Text.Length == 0) return;
            if (txtEmployeeName.Text.Length == 0) return;
            if (txtQty.Text.Length == 0) return;

            Guid guidID = Guid.NewGuid();
            DataRowView row = cbox_Meals.SelectedItem as DataRowView;
            clsLunch lunch = new clsLunch();
            clsTicketDetail itemDetail = new clsTicketDetail();
            List<clsTicketDetail> itemDetailList = new List<clsTicketDetail>();

            lunch.LunchDate = workDay;
            lunch.GUID = guidID.ToString();
            lunch.EmployeeName = txtEmployeeName.Text.ToUpper();
            lunch.Qty = Convert.ToInt32(txtQty.Text);
            lunch.MealID = Convert.ToInt32(row["ID"]);

            clsItem item = DB.GetItem(lunch.MealID);

            itemDetail.GUID = lunch.GUID;
            itemDetail.Qty = lunch.Qty;
            itemDetail.ItemID = lunch.MealID;
            itemDetail.ItemDesc = row["ItemDescription"].ToString();
            itemDetail.UnitPrice = item.UnitPrice;
            itemDetail.TotalPrice = item.UnitPrice * lunch.Qty;
            itemDetail.UnitCost = item.UnitCost;
            itemDetail.TotalCost = item.UnitCost * lunch.Qty;

            itemDetailList.Add(itemDetail);

            if (DB.InsertLunch(lunch))
            {
                if (DB.InsertTicketDetail(itemDetailList, lunch.GUID, Settings.Default.WhoOpen, false))
                {
                    Helper.GetMealItemsFromTicket(lunch.EmployeeName, itemDetailList);
                    Helper.ShowToastNotification($"{strLunchAdded}");
                }
            }

            CleanAll("Lunch");
        }
        #endregion EMPOYEES LUNCH

        #region SALARY ADVANCES
        private void txtAdvAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtAdvAmount.Text = numKey.numKeyed;
            AddAdv.IsEnabled = true;
        }

        private void btn_AddAdv(object sender, RoutedEventArgs e)
        {
            if (AdvDatePicker.Text.Length == 0) return;
            if (txtAdvRequester.Text.Length == 0) return;
            if (txtAdvAmount.Text.Length == 0) return;

            clsSalaryAdvance salAdvance = new clsSalaryAdvance();
            salAdvance.BusinessDate = Settings.Default.BusinessDate;
            salAdvance.Requester = txtAdvRequester.Text;
            salAdvance.Approver = Settings.Default.WhoOpen;
            salAdvance.Amount = Convert.ToInt32(txtAdvAmount.Text);

            if (DB.InsertSalaryAdvance(salAdvance))
            {
                Helper.ShowToastNotification($"Adelanto de salario ok");
            }

            CleanAll("Advance");
        }
        #endregion
    }
}
