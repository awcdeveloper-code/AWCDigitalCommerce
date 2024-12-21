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
using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfQryGraphics.xaml
    /// </summary>
    public partial class wpfQryGraphics : Window
    {
        private TabItem ucNewTab;

        public wpfQryGraphics()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Columns
            var ucItemType1 = new ucItemType1();
            ucNewTab = new TabItem { Content = ucItemType1 };
            ucNewTab.Header = "COLUMNAR";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Bars
            var ucItemType2 = new ucItemType2();
            ucNewTab = new TabItem { Content = ucItemType2 };
            ucNewTab.Header = "BARRAS";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Lines
            var ucItemType3 = new ucItemType3();
            ucNewTab = new TabItem { Content = ucItemType3 };
            ucNewTab.Header = "LINEAR";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Area
            var ucItemType4 = new ucItemType4();
            ucNewTab = new TabItem { Content = ucItemType4 };
            ucNewTab.Header = "AREA";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Pie
            var ucItemType5 = new ucItemType5();
            ucNewTab = new TabItem { Content = ucItemType5 };
            ucNewTab.Header = "TAJADAS";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Scatter
            var ucItemType6 = new ucItemType6();
            ucNewTab = new TabItem { Content = ucItemType6 };
            ucNewTab.Header = "DISPERSIÓN";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            // Bubbles
            var ucItemType7 = new ucItemType7();
            ucNewTab = new TabItem { Content = ucItemType7 };
            ucNewTab.Header = "CIRCULAR";
            ucNewTab.FontSize = 12;
            ucNewTab.FontWeight = FontWeights.DemiBold;

            WorkArea.Items.Add(ucNewTab);

            WorkArea.Items.Refresh();
            WorkArea.Visibility = Visibility.Visible;
        }
    }
}
