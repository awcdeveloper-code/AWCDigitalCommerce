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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Waitrest
{
    /// <summary>
    /// Interaction logic for ucTicketDashboard.xaml
    /// </summary>
    public partial class ucTicketDashboard : UserControl
    {
        public ucTicketDashboard()
        {
            InitializeComponent();
        }

        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Increase.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;
            Decrease.Visibility = Visibility.Visible;
        }

        private void btn_Increase(object sender, MouseButtonEventArgs e)
        {

        }

        private void btn_Delete(object sender, MouseButtonEventArgs e)
        {

        }

        private void btn_Decrease(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
