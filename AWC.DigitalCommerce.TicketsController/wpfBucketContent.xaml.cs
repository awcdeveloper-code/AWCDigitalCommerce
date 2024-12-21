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
    public partial class wpfBucketContent : Window
    {
        public wpfBucketContent(int ID)
        {
            InitializeComponent();

            List<clsItem> itemsList = DB.GetBucketItemsListByTicketNumber(ID);
            BucketContent.ItemsSource = itemsList;
        }

        private void wpfBucketContent_ContentRendered(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
