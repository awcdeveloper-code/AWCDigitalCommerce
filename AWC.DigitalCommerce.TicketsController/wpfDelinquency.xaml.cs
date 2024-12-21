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
    /// Interaction logic for wpfDelinquency.xaml
    /// </summary>
    public partial class wpfDelinquency : Window
    {
        private string yearMonth = string.Empty;
        private List<clsDelincuency> delincuenciesList = new List<clsDelincuency>();
        public wpfDelinquency()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            cbox_Month.Items.Add("TODOS");
            cbox_Month.Items.Add("ENERO");
            cbox_Month.Items.Add("FEBRERO");
            cbox_Month.Items.Add("MARZO");
            cbox_Month.Items.Add("ABRIL");
            cbox_Month.Items.Add("MAYO");
            cbox_Month.Items.Add("JUNIO");
            cbox_Month.Items.Add("JULIO");
            cbox_Month.Items.Add("AGOSTO");
            cbox_Month.Items.Add("SETIEMBRE");
            cbox_Month.Items.Add("OCTUBRE");
            cbox_Month.Items.Add("NOVIEMBRE");
            cbox_Month.Items.Add("DICIEMBRE");
        }

        private void cbox_Month_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_Month.SelectedIndex == -1) return;

            if (cbox_Month.SelectedIndex == 0)  // all
                yearMonth = DateTime.Now.ToString("yyyy");
            else
                yearMonth = DateTime.Now.ToString("yyyy") + cbox_Month.SelectedIndex.ToString("00");

            delincuenciesList = DB.GetDelincuencies(yearMonth + "%");
            dgDelincuency.ItemsSource = delincuenciesList;

            Print.IsEnabled = dgDelincuency.Items.Count > 0;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(delincuenciesList);
        }
    }
}
