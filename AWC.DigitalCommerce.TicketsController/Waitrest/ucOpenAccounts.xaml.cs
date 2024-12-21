using AWC.DigitalCommerce.TicketsController.Controls;
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
    /// Interaction logic for ucOpenAccounts.xaml
    /// </summary>
    public partial class ucOpenAccounts : UserControl
    {
        private class oa
        {
            public string CustomerAKA { get; set; }
            public int TotalPrice { get; set; }
        }

        public ucOpenAccounts()
        {
            InitializeComponent();

            for (int i = 1; i <= 20; i++)
            {
                oa openacc = new oa();
                openacc.CustomerAKA = $"TICKET #{i}";
                openacc.TotalPrice = 15500;
                OpenAccounts.Items.Add(openacc);
            }
        }

        private void ClickOn_PrintTicket(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickOn_PrintTicket");
        }

        private void ClickOn_Options(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickOn_Options");
        }

        private void ClickOn_PayTicket(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickOn_PayTicket");
        }

        private void OpenAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("OpenAccounts_SelectionChanged");
        }
    }
}
