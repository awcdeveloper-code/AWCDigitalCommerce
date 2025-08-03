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
    /// Interaction logic for wpfSalesDashboard.xaml
    /// </summary>
    public partial class wpfSalesDashboard : Window
    {
        public wpfSalesDashboard()
        {
            InitializeComponent();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_search(object sender, RoutedEventArgs e)
        {

        }

        private void btn_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
