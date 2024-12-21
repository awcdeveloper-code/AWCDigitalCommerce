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
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfChangeCustName.xaml
    /// </summary>
    public partial class wpfChangeCustName : Window
    {
        public bool bCancel = false;
        public bool restVoucher = false;
        public string newName = string.Empty;
        public wpfChangeCustName()
        {
            InitializeComponent();
        }

        private void txtNewName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnOK.IsEnabled = txtNewName.Text.Length >= 2 ? true : false;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            bCancel = true;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            restVoucher = chkBox_RestVoucher.IsChecked.Value;
            newName = txtNewName.Text.ToUpper();
            this.Close();
        }
    }
}
