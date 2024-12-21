using AWC.DigitalCommerce.TicketsController.Properties;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucIncomeCash.xaml
    /// </summary>
    public partial class ucIncomeCash : UserControl
    {
        public ucIncomeCash()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CleanAll()
        {
            txtIncomeDescription.Text = string.Empty;
            txtIncomeAmount.Text = string.Empty;
            txtIncomeDescription.Focus();
        }

        private void txtIncomeAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtIncomeAmount.Text = numKey.numKeyed;
            Add.IsEnabled = true;
        }
        private void btn_AddIncome(object sender, RoutedEventArgs e)
        {
            int incomeAmount = Convert.ToInt32(txtIncomeAmount.Text);
            string msg = $"Ingreso de {incomeAmount} colones en efectivo OK";
            //Settings.Default.CashRegisterOpening += incomeAmount;
            //Settings.Default.Save();
            DB.InsertIncome(txtIncomeDescription.Text.ToUpper(), incomeAmount);
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, msg, Logger.Severity.ERROR);
            Helper.ShowToastNotification($"{msg}");
            CleanAll();
        }
    }
}
