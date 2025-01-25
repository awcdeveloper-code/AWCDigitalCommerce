using AWC.DigitalCommerce.POSitive.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.POSitive
{
    public partial class MainWindow : Window
    {
        // GLOBAL VARIABLES
        public string AppName = "AWC POSitive 2005";
        public MainWindow()
        {
            InitializeComponent();
            this.Title = AppName;

            var UC = new ucWelcome();
            TabItem newTab = new TabItem { Content = UC };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/G3_Logo.ico", "BIENVENIDO");

            tabCtrlWorkArea.Items.Add(newTab);
            tabCtrlWorkArea.Items.Refresh();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Realmente desea salir de la sesión (S/N)?", AppName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Option10_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Realmente desea salir de la sesión (S/N)?", AppName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        #region UTILITIES
        private StackPanel CreateHeaderForTabItem(TabItem newTab, string uriImage, string header)
        {
            StackPanel headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(uriImage));
            image.Width = 30;
            image.Height = 30;
            image.Margin = new Thickness(5, 0, 5, 0);
            headerPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = header;
            textBlock.VerticalAlignment = VerticalAlignment.Center;

            headerPanel.Children.Add(textBlock);

            newTab.FontSize = 15;
            newTab.FontWeight = FontWeights.DemiBold;

            return headerPanel;
        }
        #endregion
    }
}
