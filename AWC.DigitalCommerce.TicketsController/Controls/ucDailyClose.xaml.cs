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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucDailyClose.xaml
    /// </summary>
    public partial class ucDailyClose : UserControl
    {
        private string lang = string.Empty;
        private TabItem newTab = new TabItem();
        private Image img = new Image();

        public ucDailyClose(string _lang)
        {
            lang = _lang;
            InitializeComponent();

            Mouse.OverrideCursor = Cursors.Wait;

            var DailyClosing_RestoBar = new ucDailyClosing_RestoBar(lang);
            newTab = new TabItem { Content = DailyClosing_RestoBar };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/food-service.png", "RESTOBAR");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Beverages = new ucDailyClosing_Beverages(lang);
            newTab = new TabItem { Content = DailyClosing_Beverages };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/beer.png", "BEBIDAS");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Liquors = new ucDailyClosing_Liquors(lang);
            newTab = new TabItem { Content = DailyClosing_Liquors };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/liquors.ico", "LICORES");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Kitchen = new ucDailyClosing_Kitchen(lang);
            newTab = new TabItem { Content = DailyClosing_Kitchen };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/kitchen.ico", "COCINA");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Expenses = new ucExpensesReport(lang);
            newTab = new TabItem { Content = DailyClosing_Expenses };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/expenses.png", "GASTOS INTERNOS");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_IncomeCash = new ucIncomeCash();
            newTab = new TabItem { Content = DailyClosing_IncomeCash };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/Money.ico", "INGRESOS A CAJA");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Comsuption = new ucDailyClosing_Comsuption(lang);
            newTab = new TabItem { Content = DailyClosing_Comsuption };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/restaurant.png", "CONSUMOS");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_AccountsReceivable = new ucDailyClosing_AccountsReceivable(lang);
            newTab = new TabItem { Content = DailyClosing_AccountsReceivable };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/a2p.png", "PENDIENTES");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_SalesHistory = new ucDailyClosing_SalesHistory(lang);
            newTab = new TabItem { Content = DailyClosing_SalesHistory };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/salesHistory.png", "HISTORIAL DE VENTAS");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_Graphics = new ucDailyClosing_Graphics(lang);
            newTab = new TabItem { Content = DailyClosing_Graphics };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/graphics.png", "GRÁFICOS");
            DailyClosingTabControl.Items.Add(newTab);

            var ucReportsInventory = new ucReportsInventoryMgmt();
            newTab = new TabItem { Content = ucReportsInventory };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/storage.png", "REPORTES");
            DailyClosingTabControl.Items.Add(newTab);

            var ucDailyClosingItemsByUser = new ucDailyClosing_ItemsByUser();
            newTab = new TabItem { Content = ucDailyClosingItemsByUser };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/waitress.png", "VENTAS POR COLABORADOR");
            DailyClosingTabControl.Items.Add(newTab);

            var DailyClosing_ServiceFee = new ucDailyClosing_ServiceFeeByWho(lang);
            newTab = new TabItem { Content = DailyClosing_ServiceFee };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/discount.png", "10% SERVICIO");
            DailyClosingTabControl.Items.Add(newTab);

            var ucInternalOrders = new ucInternalOrders();
            newTab = new TabItem { Content = ucInternalOrders };
            newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/email.png", "PEDIDOS");
            DailyClosingTabControl.Items.Add(newTab);

            if (Settings.Default.RestoBarAdvActive)
            {
                var DailyClosing_RestoBarAdv = new ucDailyClosing_RestoBarAdv(lang);
                newTab = new TabItem { Content = DailyClosing_RestoBarAdv };
                newTab.Header = "RESTOBAR ADV";
                newTab.FontSize = 20;
                newTab.FontWeight = FontWeights.DemiBold;
                DailyClosingTabControl.Items.Add(newTab);
            }

            DailyClosingTabControl.Items.Refresh();
            DailyClosingTabControl.Visibility = Visibility.Visible;

            Mouse.OverrideCursor = null;
        }
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
    }
}
