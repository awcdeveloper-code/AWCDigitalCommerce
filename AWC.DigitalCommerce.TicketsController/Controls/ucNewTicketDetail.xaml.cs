using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ucNewTicketDetail : UserControl
    {
        private wpfMainWindow mw;
        private static ucNewTicketDetail localucNewTicketDetail;
        private static string fullLogPath = string.Empty;
        private static string fullLogFileName = string.Empty;
        private string lang = string.Empty;
        private List<clsTicketDetail> itemsDetail = new List<clsTicketDetail>();
        private List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        private Dictionary<int, int> itemsIDList = new Dictionary<int, int>();

        public ucNewTicketDetail(wpfMainWindow _mw, string _lang)
        {
            mw = _mw;

            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            localucNewTicketDetail = this;

            InitializeItemsDetailCache();

            Cancel.Focus();
        }
        private void InitializeItemsDetailCache()
        {
            fullLogPath = System.IO.Path.Combine(Settings.Default.SerilogRootPath, "WorkArea");

            if (!Directory.Exists(fullLogPath))
                Directory.CreateDirectory(fullLogPath);

            fullLogFileName = System.IO.Path.Combine(fullLogPath, "TicketDetail.tmp");

            if (File.Exists(fullLogFileName))
                File.Delete(fullLogFileName);
        }
        private Dictionary<int, int> LoadCacheInMemory()
        {
            Dictionary<int, int> tmpDict = new Dictionary<int, int>();

            using (StreamReader sr = new System.IO.StreamReader(fullLogFileName))
            {
                while (!sr.EndOfStream)
                {
                    string rec = sr.ReadLine();

                    clsTicketDetail rdi = new clsTicketDetail();

                    rdi.ItemID = Convert.ToInt32(rec.Split('|')[0]);
                    rdi.GUID = rec.Split('|')[1];
                    rdi.ItemDesc = rec.Split('|')[2];
                    rdi.Qty = Convert.ToInt32(rec.Split('|')[3]);
                    rdi.UnitCost = Convert.ToInt32(rec.Split('|')[4]);
                    rdi.TotalCost = Convert.ToInt32(rec.Split('|')[5]);
                    rdi.UnitPrice = Convert.ToInt32(rec.Split('|')[6]);
                    rdi.TotalPrice = Convert.ToInt32(rec.Split('|')[7]);

                    tmpDict.Add(rdi.ItemID, rdi.Qty);
                }
            }
            return tmpDict;
        }
        private List<clsTicketDetail> ExtractNewMealsOrder(Dictionary<int, int> tmpDict)
        {
            List < clsTicketDetail > tmp = new List<clsTicketDetail>();

            foreach (clsTicketDetail rdi in TicketDetail.Items)
            {
                if (DB.IsMealItemType(rdi.ItemDesc))
                {
                    if (rdi.Qty > tmpDict[rdi.ItemID])
                    {
                        clsTicketDetail newMealOrder = new clsTicketDetail();

                        newMealOrder.ItemID = rdi.ItemID;
                        newMealOrder.GUID = rdi.GUID;
                        newMealOrder.ItemDesc = rdi.ItemDesc;
                        newMealOrder.Qty = rdi.Qty - tmpDict[rdi.ItemID];
                        newMealOrder.UnitCost = rdi.UnitCost;
                        newMealOrder.TotalCost = rdi.TotalCost;
                        newMealOrder.UnitPrice = rdi.UnitPrice;
                        newMealOrder.TotalPrice = rdi.TotalPrice;
                        newMealOrder.Note = rdi.Note;

                        tmp.Add(newMealOrder);
                    }
                }
            }
            return tmp;
        }
        public static void ReceiveDataFromNewTicket(clsTicket data)
        {

        }
        public static void ReceiveDataFromNewTicketDetail(clsTicketDetail data)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullLogFileName, true))
            {
                string itemDetail = data.ItemID + "|" +
                                    data.GUID + "|" +
                                    data.ItemDesc + "|" +
                                    data.Qty + "|" +
                                    data.UnitCost + "|" +
                                    data.TotalCost + "|" +
                                    data.UnitPrice + "|" +
                                    data.TotalPrice;
                sw.WriteLine(itemDetail);
                sw.Flush();
            }

            clsTicketDetail rdi = new clsTicketDetail();

            rdi.ItemID = data.ItemID;
            rdi.GUID = data.GUID;
            rdi.ItemDesc = data.ItemDesc;
            rdi.Qty = data.Qty;
            rdi.UnitCost = data.UnitCost;
            rdi.TotalCost = data.TotalCost;
            rdi.UnitPrice = data.UnitPrice;
            rdi.TotalPrice = data.TotalPrice;

            localucNewTicketDetail.TicketNumber.Content = Helper.Ticket.ID.ToString("000000");
            localucNewTicketDetail.CustomerID.Content = Helper.CustomerID;

            localucNewTicketDetail.TicketDetail.Items.Add(rdi);
            localucNewTicketDetail.TicketDetail.Items.Refresh();
        }
        public static void CleanTicketDetailDataGrid()
        {
            try
            {
                localucNewTicketDetail.TicketDetail.Items.Clear();
                localucNewTicketDetail.TicketDetail.Items.Refresh();
            }
            catch { }
        }
        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Increase.Visibility = Visibility.Visible;
            Decrease.Visibility = Visibility.Visible;
        }
        private void btn_Increase(object sender, MouseButtonEventArgs e)
        {
            foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
            {
                rdi.Qty++;
                rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;

                if (DB.IsMealItemType(rdi.ItemDesc))
                {
                    wpfMealNote mn = new wpfMealNote(rdi.ItemDesc);
                    mn.ShowDialog();
                    rdi.Note = mn.mealNote;
                }
            }

            TicketDetail.Items.Refresh();

            Cancel.IsEnabled = true;
            UpdateTicket.IsEnabled = true;
            mw.UpdateTicket.IsEnabled = false;
        }
        private void btn_Decrease(object sender, MouseButtonEventArgs e)
        {
            foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
            {
                if (rdi.Qty == 1)
                {
                    wpfMessageBox.Show("Ticket Controller", "CANTIDAD DE UN ITEM NO PUEDE SER CERO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    continue;
                }
                else
                {
                    rdi.Qty--;
                    rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;
                }
            }

            TicketDetail.Items.Refresh();

            Cancel.IsEnabled = true;
            UpdateTicket.IsEnabled = true;
            mw.UpdateTicket.IsEnabled = false;
        }
        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            TicketDetail.Items.Clear();

            using (StreamReader sr = new System.IO.StreamReader(fullLogFileName))
            {
                while (!sr.EndOfStream)
                {
                    string rec = sr.ReadLine();

                    clsTicketDetail rdi = new clsTicketDetail();

                    rdi.ItemID = Convert.ToInt32(rec.Split('|')[0]);
                    rdi.GUID = rec.Split('|')[1];
                    rdi.ItemDesc = rec.Split('|')[2];
                    rdi.Qty = Convert.ToInt32(rec.Split('|')[3]);
                    rdi.UnitCost = Convert.ToInt32(rec.Split('|')[4]);
                    rdi.TotalCost = Convert.ToInt32(rec.Split('|')[5]);
                    rdi.UnitPrice = Convert.ToInt32(rec.Split('|')[6]);
                    rdi.TotalPrice = Convert.ToInt32(rec.Split('|')[7]);

                    TicketDetail.Items.Add(rdi);
                }
            }

            TicketDetail.Items.Refresh();

            Increase.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;
            UpdateTicket.IsEnabled = true;
        }
        private void btn_UpdateTicket(object sender, RoutedEventArgs e)
        {
            itemsIDList = LoadCacheInMemory();

            InitializeItemsDetailCache();

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullLogFileName, true))
            {
                itemsDetail.Clear();

                foreach (clsTicketDetail rdi in TicketDetail.Items)
                {
                    string itemDetail = rdi.ItemID + "|" +
                                        rdi.GUID + "|" +
                                        rdi.ItemDesc + "|" +
                                        rdi.Qty + "|" +
                                        rdi.UnitCost + "|" +
                                        rdi.TotalCost + "|" +
                                        rdi.UnitPrice + "|" +
                                        rdi.TotalPrice;
                    sw.WriteLine(itemDetail);
                    sw.Flush();

                    itemsDetail.Add(rdi);
                }
            }

            DB.DeleteTicketDetail(Helper.Ticket.GUID, true);

            if (DB.InsertTicketDetail(itemsDetail, Helper.Ticket.GUID, Settings.Default.WhoOpen, true))
            {
                if (Settings.Default.PrintOrder)
                    Helper.PrintTicket(Helper.CustomerID, itemsDetail);

                newMealsOrder = ExtractNewMealsOrder(itemsIDList);

                if (newMealsOrder.Count > 0)
                    Helper.GetMealItemsFromTicket(Helper.Ticket.CustID, newMealsOrder);
            }

            wpfSplashWindow swnd = new wpfSplashWindow(1, lang);
            swnd.ShowDialog();

            TicketDetail.UnselectAll();
            TicketDetail.Items.Refresh();

            Increase.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;

            UpdateTicket.IsEnabled = false;
            mw.UpdateTicket.IsEnabled = true;

            Cancel.IsEnabled = false;
            Cancel.Focus();
        }
    }
}
