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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSendReporsByEMail.xaml
    /// </summary>
    public partial class wpfSendReportByEMail : Window
    {
        private string sd = string.Empty;
        private string ed = string.Empty;

        public wpfSendReportByEMail()
        {
            InitializeComponent();

            cbox_ReportList.Items.Add("LISTA DE PRECIOS");
            cbox_ReportList.Items.Add("ESTADO DEL INVENTARIO");
            cbox_ReportList.Items.Add("LISTA DE CLIENTES");
            cbox_ReportList.Items.Add("CIERRES DIARIOS");
            cbox_ReportList.Items.Add("CUENTAS DIARIAS");
            cbox_ReportList.Items.Add("CUENTAS ANULADAS");
            cbox_ReportList.Items.Add("CONSUMO COMPLETO");
            cbox_ReportList.Items.Add("INGRESOS/SALIDAS DE EMPLEADOS");
            cbox_ReportList.Items.Add("APERTURA DE CAJÓN DE DINERO");

            txtBox_eMail.Text = Settings.Default.eMailDistributionList;
        }

        private void cbox_ReportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sd = string.Empty;
            ed = string.Empty;

            if (cbox_ReportList.SelectedIndex < 3)
            {
                StartDay.IsEnabled = false;
                EndDay.IsEnabled = false;
            }
            else
            {
                StartDay.IsEnabled = true;
                EndDay.IsEnabled = true;
            }
        }

        private void StartDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            sd = StartDay.SelectedDate.ToString();

            if (sd.Length == 0) return;

            string year = sd.Split('/')[2].Substring(0, 4);
            string month = sd.Split('/')[1].PadLeft(2, '0');
            string day = sd.Split('/')[0].PadLeft(2, '0');

            sd = year + month + day;
        }

        private void EndDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ed = EndDay.SelectedDate.ToString();

            if (ed.Length == 0) return;

            string year = ed.Split('/')[2].Substring(0, 4);
            string month = ed.Split('/')[1].PadLeft(2, '0');
            string day = ed.Split('/')[0].PadLeft(2, '0');

            ed = year + month + day;

            if (Convert.ToInt32(sd) > Convert.ToInt32(ed))
            {
                wpfMessageBox.Show("Tickets Controller", "FECHA INICIAL NO PUEDE SER MAYOR QUE LA FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                StartDay.SelectedDate = null;
                EndDay.SelectedDate = null;
                return;
            }

            btnSend.Focus();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Send(object sender, RoutedEventArgs e)
        {
            if (cbox_ReportList.SelectedIndex > 2 && (sd.Length == 0 || ed.Length == 0))
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EL REPORTE SELECCIONADO REQUIERE FECHA INICIAL Y FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
                return;
            }

            if (!SMTP.CheckInternetConnection())
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EN ESTE MOMENTO NO HAY CONEXIÓN A INTERNET, POR FAVOR INTENTE MAS TARDE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, null);
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            SMTP.SendReportByEMail(cbox_ReportList.SelectedIndex, txtBox_eMail.Text, sd, ed);
            Mouse.OverrideCursor = null;

            Helper.ShowToastNotification("ATENCIÓN: CORREO ENVIADO");
        }
    }
}
