using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public class dgItems    // just for local use
    {
        public int ID { get; set; }
        public int ItemType { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQty { get; set; }
        public int ItemPrice { get; set; }
        public int ItemTotal { get; set; }
        public string GUID { get; set; }
    }

    public partial class ucNotes : System.Windows.Controls.UserControl
    {
        private string noteDate = string.Empty;
        private clsItem selectedItem = new clsItem();
        private List<clsItem> itemsList = new List<clsItem>();
        private int total = 0;
        public ucNotes()
        {
            InitializeComponent();

            itemsList = DB.ListBinding_tbl_Items(0);
            cbox_ItemsList.ItemsSource = itemsList;
            total = 0;
        }

        private void CleanAll()
        {            
            total = 0;
            NoteDate.Text = string.Empty;
            DebitNote.IsChecked = false;
            CreditNote.IsChecked = false;
            txtNoteDescription.Text = string.Empty;
            cbox_ItemsList.SelectedIndex = -1;
            txtItemQty.Text = string.Empty;
            btnAddItem.IsEnabled = false;
            dgItemsList.Items.Clear();
            btnDeleteItem.IsEnabled = false;
            btnSaveNote.IsEnabled = false;
            NoteDate.Focus();
        }

        private void NoteDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            noteDate = NoteDate.SelectedDate.ToString();

            if (noteDate.Length == 0) return;

            string year = noteDate.Split('/')[2].Substring(0, 4);
            string month = noteDate.Split('/')[1].PadLeft(2, '0');
            string day = noteDate.Split('/')[0].PadLeft(2, '0');

            noteDate = year + month + day;
        }

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbox_ItemsList.SelectedIndex >= 0 && txtItemQty.Text.Length > 0)
                btnAddItem.IsEnabled = true;
            else
                btnAddItem.IsEnabled = false;
        }

        private void btn_AddItem(object sender, RoutedEventArgs e)
        {
            clsItem item = cbox_ItemsList.SelectedItem as clsItem;

            dgItems addItem2dg = new dgItems();

            addItem2dg.ID = item.ID;
            addItem2dg.ItemType = item.ItemType;
            addItem2dg.ItemDescription = item.ItemDescription;
            addItem2dg.ItemQty = Convert.ToInt32(txtItemQty.Text);
            addItem2dg.ItemPrice = item.UnitPrice;
            addItem2dg.ItemTotal = addItem2dg.ItemQty * addItem2dg.ItemPrice;

            total += addItem2dg.ItemTotal;

            dgItemsList.Items.Add(addItem2dg);

            cbox_ItemsList.Text = string.Empty;
            txtItemQty.Text = string.Empty;

            btnDeleteItem.IsEnabled = true;
            btnSaveNote.IsEnabled = true;
        }

        private void dgItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItemsList.Items.Count > 0)
            {
                btnSaveNote.IsEnabled = true;
                btnDeleteItem.IsEnabled = true;
            }
            else
            {
                btnSaveNote.IsEnabled = true;
                btnDeleteItem.IsEnabled = true;
            }
        }

        private void btn_DeleteItem(object sender, RoutedEventArgs e)
        {
            clsNoteDetail selectedItem = dgItemsList.SelectedItem as clsNoteDetail;

            if (selectedItem != null)
            {
                dgItemsList.Items.Remove(selectedItem);
                total -= selectedItem.ItemTotal;
            }

            btnDeleteItem.IsEnabled = false;
        }

        private void btn_SaveNote(object sender, RoutedEventArgs e)
        {
            try
            {
                clsNote newNote = new clsNote();

                newNote.NoteDate = noteDate;

                if (DebitNote.IsChecked == true)
                    newNote.NoteType = 0;
                else
                    newNote.NoteType = 1;

                newNote.NoteDescription = txtNoteDescription.Text.ToUpper();

                Guid guidID = Guid.NewGuid();
                newNote.NoteGUID = guidID.ToString();

                newNote.NoteAmount = total;

                DB.InsertNewNote(newNote);

                Logger.WriteToLog("InventoriesManagement", "Note [" + newNote.NoteDescription + "] added.", Logger.Severity.INFORMATION);

                foreach (dgItems item in dgItemsList.Items)
                {
                    // prepare the record
                    clsNoteDetail invItem = new clsNoteDetail();

                    invItem.NoteGUID = newNote.NoteGUID;
                    invItem.ItemType = item.ItemType;
                    invItem.ItemID = item.ID;
                    invItem.ItemQty = item.ItemQty;
                    invItem.ItemPrice = item.ItemPrice;
                    invItem.ItemTotal = item.ItemTotal;

                    // add invoice item
                    DB.InsertNewNoteDetail(invItem);

                    // update item inventory
                    clsItem workItem = DB.GetItem(item.ID);

                    clsItem updItem = new clsItem();
                    updItem.ID = item.ID;
                    updItem.ItemSold = item.ItemQty * workItem.ItemUnitSize;

                    if (newNote.NoteType == 0)
                        DB.UpdateItemInventory("DEB", updItem); // DEBIT
                    else
                        DB.UpdateItemInventory("CRED", updItem); // CREDIT
                }

                Logger.WriteToLog("InventoriesManagement", "Items of invoice [" + newNote.NoteDescription + "] added.", Logger.Severity.DEBUG);

                CleanAll();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }
    }
}
