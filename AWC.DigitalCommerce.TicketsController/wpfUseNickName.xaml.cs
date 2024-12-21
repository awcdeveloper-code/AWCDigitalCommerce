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
    /// Interaction logic for wpfUseNickName.xaml
    /// </summary>
    public partial class wpfUseNickName : Window
    {
        public string nickName = string.Empty;
        public wpfUseNickName(string _nickName, bool cancelButton)
        {
            this.Topmost = true;
            nickName = _nickName;
            InitializeComponent();
            Cancel.IsEnabled = cancelButton;
            Cancel.Visibility = cancelButton ? Visibility.Visible : Visibility.Hidden; ;
            txtNickName.Text = nickName.ToUpper();
            txtNickName.CaretIndex = txtNickName.Text.Length;
            txtNickName.Focus();
        }

        private bool CheckNickName()
        {
            nickName = txtNickName.Text.ToUpper().Replace(";", "Ñ");

            if (DB.GetCustomerIDFromOpenTickets(nickName))
            {
                wpfMessageBox.Show("Ticket Controller", "IDENTIFICADOR YA HA SIDO ASIGNADO. POR FAVOR, INTENTE CON OTRO DIFERENTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                return true;
            }
            return false;
        }

        private void txtNickName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtNickName.Text.Length == 0) return;

                if (CheckNickName())
                    return;

                this.Close();
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (txtNickName.Text.Length == 0) return;

            if (CheckNickName())
                return;
            
            this.Close();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            nickName = string.Empty;
            this.Close();
        }
    }
}
