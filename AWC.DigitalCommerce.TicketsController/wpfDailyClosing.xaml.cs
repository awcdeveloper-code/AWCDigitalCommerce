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
    /// Interaction logic for wpfDailyClosing.xaml
    /// </summary>
    public partial class wpfDailyClosing : Window
    {
        private string workDay = Settings.Default.BusinessDate;
        private string today = DateTime.Now.ToString("dd-MM-yyyy");
        public bool IsDailyClosing { get; set; }
        
        public wpfDailyClosing()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            lblWorkDay.Content = workDay.Substring(6, 2) + "-" + workDay.Substring(4, 2) + "-" + workDay.Substring(0, 4);
            lblToday.Content = today;
            btnYES.Focus();
        }

        private void btn_YES(object sender, RoutedEventArgs e)
        {
            IsDailyClosing = true;
            this.Close();
        }

        private void btn_NO(object sender, RoutedEventArgs e)
        {
            IsDailyClosing = false;
            this.Close();
        }
    }
}
