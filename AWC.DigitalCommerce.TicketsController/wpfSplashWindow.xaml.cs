using SwiftExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfSplashWindow : Window
    {
        private DispatcherTimer splashTimer = new DispatcherTimer();
        private string lang = string.Empty;

        // eMail thread
        private DataGrid TodayTickets = new DataGrid();
        private int option = 0;
        private string mailAddress = string.Empty;
        private int customerID = 0;

        public wpfSplashWindow(int sleep, string _lang)
        {
            this.Topmost = true;

            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            splashTimer.Tick += new EventHandler(splashTimer_Tick);
            splashTimer.Interval = new TimeSpan(0, 0, sleep);
            splashTimer.Start();
        }

        // eMail Thread
        public wpfSplashWindow(string _lang, DataGrid _TodayTickets, int _option, string _mailAddress, int _customerID)
        {
            this.Topmost = true;

            lang = _lang;
            TodayTickets= _TodayTickets;
            option = _option;
            mailAddress = _mailAddress;
            customerID = _customerID;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);
        }

        private void splashTimer_Tick(object sender, EventArgs eArgs)
        {
            splashTimer.Stop();
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // eMail Thread
            if (mailAddress.Length > 0)
            {
                DB.UpdateCustomerMailAddress(customerID, mailAddress);

                foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
                {
                    Helper.PrintTicket(item, option);
                    SMTP.EMailTicket(item, mailAddress);
                }
            }

            splashTimer.Tick += new EventHandler(splashTimer_Tick);
            splashTimer.Interval = new TimeSpan(0, 0, 1);
            splashTimer.Start();
        }
    }
}
