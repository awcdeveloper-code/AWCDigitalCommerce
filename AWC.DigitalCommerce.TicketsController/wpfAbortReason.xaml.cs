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
    /// Interaction logic for wpfAbortReason.xaml
    /// </summary>
    public partial class wpfAbortReason : Window
    {
        public string abortReason = string.Empty;

        public wpfAbortReason()
        {
            InitializeComponent();
            txtAbortReason.Focus();
        }

        private void AbortReason_KeyUp(object sender, KeyEventArgs e)
        {
            btnOK.IsEnabled = Helper.CharsInText(txtAbortReason.Text, 15);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            abortReason = string.Empty;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (txtAbortReason.Text.Length == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: DEBE DE INGRESAR LA RAZÓN POR LA CUAL ESTA ANULANDO ESTA CUENTA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                return;
            }

            abortReason = txtAbortReason.Text.ToUpper();

            this.Close();
        }
    }
}
