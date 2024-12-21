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
    /// Interaction logic for wpfAWCSplashScreen.xaml
    /// </summary>
    public partial class wpfAWCSplashScreen : Window
    {
        public wpfAWCSplashScreen()
        {
            InitializeComponent();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Helper.PrintBusinessCard();
            this.Close();
        }
        private void btn_OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
