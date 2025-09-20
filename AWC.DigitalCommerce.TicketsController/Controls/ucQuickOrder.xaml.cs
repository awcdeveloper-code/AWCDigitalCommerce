using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucQuickOrder : UserControl
    {
        #region GLOBAL VARIABLES
        private wpfMainWindow2 mw;
        private string lang = string.Empty;
        private List<clsItem> lstProducts = new List<clsItem>();
        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();
        private List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        private List<clsTicketDetail> newBeveragesOrder = new List<clsTicketDetail>();
        private int totalIVAFee = 0;
        private int totalPrice = 0;
        private bool MealOrderReminder = false;
        private int payementType = 0;
        public clsCustFreqItem custFreqItem = null;
        #endregion

        public ucQuickOrder(wpfMainWindow2 wnd, string _lang)
        {
            mw = wnd;

            lang = _lang;

            InitializeComponent();

            this.KeyDown += new KeyEventHandler(this_KeyDown);

            if (Settings.Default.PrintClosedTicket)
                PrintClosedTicket.IsChecked = true;

            if (Settings.Default.UltraQuickSale)
            {
                SuperQuickSale.IsChecked = true;
                PrintMeal.Visibility = Visibility.Hidden;
                Discount.Visibility = Visibility.Hidden;
                Payment.Visibility = Visibility.Hidden;

                Cash.Visibility = Visibility.Visible;
                CreditCard.Visibility = Visibility.Visible;
                Transfer.Visibility = Visibility.Visible;
            }
            else
            {
                SuperQuickSale.IsChecked = false;
                PrintMeal.Visibility = Visibility.Visible;
                Discount.Visibility = Visibility.Visible;
                Payment.Visibility = Visibility.Visible;

                Cash.Visibility = Visibility.Hidden;
                CreditCard.Visibility = Visibility.Hidden;
                Transfer.Visibility = Visibility.Hidden;
            }

            lstProducts = DB.ListBinding_tbl_Items(5);
            lBox_Products.ItemsSource = lstProducts;
            Payment.Focus();
        }

        #region UTILITIES
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.F1:
                    mw.QuickSale.IsEnabled = true;
                    mw.tabCtrlWorkArea.Items.Clear();
                    mw.tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private void CleanAll()
        {
            txtSearchProduct.Text = string.Empty;
            lBox_Products.UnselectAll();
            txtQtyProduct.Text = string.Empty;
            TicketDetail.Items.Clear();
            itemsDetails.Clear();
            totalPrice = 0;
            lblTotalSale.Content = "TOTAL: 0";
            PrintMeal.IsEnabled = false;
            Discount.IsEnabled = false;
            Payment.IsEnabled = false;
            MealOrderReminder = false;
            mw.QuickSale.IsEnabled = true;

            Increase.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;

            if (Settings.Default.UltraQuickSale && TicketDetail.Items.Count == 0)
            {
                Cash.IsEnabled = false;
                CreditCard.IsEnabled = false;
                Transfer.IsEnabled = false;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchProduct.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from item in lstProducts
                              let ename = item.ItemDescription
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select item;

            lBox_Products.ItemsSource = empFiltered;
        }
        private void txtSearchProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                txtSearchProduct.Text = alphaKey.alphaKeyed;
            }
        }
        private void lBox_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtQtyProduct.IsEnabled = true;
            txtQtyProduct.Text = "1";
            AddProduct.IsEnabled = true;
        }
        private void txtQtyProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQtyProduct.Text = numKey.numKeyed;
            btn_AddProduct(sender, e);
        }
        private void UltraQuickSale(int paymentType)
        {
            int cash = 0;
            int creditCard = 0;
            int transfer = 0;
            int voucher = 0;

            switch (paymentType)
            {
                case 1:
                    cash = totalPrice;
                    break;
                case 2:
                    creditCard = totalPrice;
                    break;
                case 3:
                    transfer = totalPrice;
                    break;
            }

            foreach (clsTicketDetail itemdg in TicketDetail.Items)
            {
                clsItem item = new clsItem();
                item.ID = itemdg.ItemID;
                item.ItemSold = itemdg.Qty;
                DB.UpdateItemInventory("SAL", item);

                if (DB.IsMealItemType(itemdg.ItemDesc))
                {
                    Helper.ApplySaleToInvenytory(itemdg.ItemID, itemdg.Qty);
                }

                if (itemdg.Bucket)
                {
                    string[] bucketContent = itemdg.Note.Split('$');

                    foreach (string idg in bucketContent)
                    {
                        clsItem ni = new clsItem();
                        ni.ID = Convert.ToInt32(idg.Split(',')[0]);
                        ni.ItemSold = Convert.ToInt32(idg.Split(',')[1]);
                        DB.UpdateItemInventory("SAL", ni);
                    }

                }

            }

            Guid guidID = Guid.NewGuid();

            int ticketNumber = DB.CreateNextTicket(guidID.ToString(), Settings.Default.QuickOrderCustID);

            DB.UpdateTicketStatus(ticketNumber, 0, 0, 0, cash, creditCard, transfer, voucher,
                                  Settings.Default.WhoOpen, DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID));

            DB.InsertTicketDetail(itemsDetails, guidID.ToString(), Settings.Default.WhoOpen, true);

            // print cancelled ticket
            if (PrintClosedTicket.IsChecked == true)
            {
                ticket.ID = ticketNumber;
                ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
                ticket.CustomerID = DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID);
                ticket.Cash = cash;
                ticket.CreditCard = creditCard;
                ticket.Transfer = transfer;
                ticket.Status = false;
                ticket.IVAFee = totalIVAFee;
                Helper.PrintTicket(ticket);
            }

            if (paymentType == 3)
            {
                if (Settings.Default.PrintSINPETicket)
                {
                    ticket.ID = ticketNumber;
                    ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
                    ticket.CustomerID = DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID);
                    ticket.Transfer = transfer;
                    Helper.PrintTicket(ticket, 1);
                }
            }

            if (Settings.Default.GenerateXMLforTicket)
            {
                Helper.GenerateXMLforTicket(ticketNumber);
            }

            if (Settings.Default.UseCashDrawer)
            {
                xPrinterOpenCashbox xpCash = new xPrinterOpenCashbox();
                xpCash.print();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Open Cash Drawer request by user {Settings.Default.WhoOpen}", Logger.Severity.WARNING);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
            sw.ShowDialog();
            CleanAll();
        }
        #endregion

        #region CHECKBOXES
        private void chkBox_PrintClosedTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintClosedTicket == false)
                Settings.Default.PrintClosedTicket = true;
            else
                Settings.Default.PrintClosedTicket = false;

            Settings.Default.Save();
        }
        private void chkBox_SuperQuickSale(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.UltraQuickSale == false)
            {
                Settings.Default.UltraQuickSale = true;
                PrintMeal.Visibility = Visibility.Hidden;
                Discount.Visibility = Visibility.Hidden;
                Payment.Visibility = Visibility.Hidden;

                Cash.Visibility = Visibility.Visible;
                CreditCard.Visibility = Visibility.Visible;
                Transfer.Visibility = Visibility.Visible;
            }
            else
            {
                Settings.Default.UltraQuickSale = false;
                PrintMeal.Visibility = Visibility.Visible;
                Discount.Visibility = Visibility.Visible;
                Payment.Visibility = Visibility.Visible;

                Cash.Visibility = Visibility.Hidden;
                CreditCard.Visibility = Visibility.Hidden;
                Transfer.Visibility = Visibility.Hidden;
            }

            if (Settings.Default.UltraQuickSale && TicketDetail.Items.Count == 0)
            {
                Cash.IsEnabled = false;
                CreditCard.IsEnabled = false;
                Transfer.IsEnabled = false;
            }
            else
            {
                Cash.IsEnabled = true;
                CreditCard.IsEnabled = true;
                Transfer.IsEnabled = true;
            }

            Settings.Default.Save();
        }
        #endregion

        #region DATAGRID
        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TicketDetail.SelectedIndex >= 0)
            {
                Increase.Visibility = Visibility.Visible;
                Delete.Visibility = Visibility.Visible;
                Decrease.Visibility = Visibility.Visible;
            }
        }
        private void TicketDetail_LostFocus(object sender, RoutedEventArgs e)
        {
            Increase.Visibility= Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;
        }
        #endregion

        #region BUTTONS
        private void btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (txtQtyProduct.Text.Trim().Length == 0) return;

            int iQtyProduct = int.Parse(txtQtyProduct.Text.Trim(), NumberStyles.Integer);

            if (iQtyProduct == 0)
            {
                wpfMessageBox.Show("Ticket Controller", "CANTIDAD NO PUEDE SER CERO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            clsItem tmp = (clsItem)lBox_Products.SelectedItem;

            clsTicketDetail ntd = new clsTicketDetail();

            ntd.ItemDesc = tmp.ItemDescription;
            ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
            ntd.ItemSubType = tmp.ItemSubType;
            ntd.Qty = Convert.ToInt32(txtQtyProduct.Text.Trim());
            ntd.UnitCost = tmp.UnitCost;
            ntd.TotalCost = ntd.UnitCost * ntd.Qty;
            ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
            ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;
            ntd.ImagePath = tmp.ImagePath;

            if (DB.IsMealItemType(tmp.ItemDescription))
            {
                this.Opacity = 0.5;
                wpfMealNote mn = new wpfMealNote(tmp.ItemDescription);
                mn.ShowDialog();
                this.Opacity = 1;

                ntd.Note = mn.mealNote;
                PrintMeal.IsEnabled = true;
                MealOrderReminder = true;
                Helper.ShowToastNotification("Recuerde imprimir la comanda de cocina");
            }

            if (DB.GetItemSubtype(ntd.ItemDesc) == 2)
            {
                this.Opacity = 0.5;
                wpfSelectBucketContent mn = new wpfSelectBucketContent(ntd.ItemID);
                mn.ShowDialog();
                this.Opacity = 1;
                ntd.Note = mn.bucketContent;
                ntd.Bucket = true;
            }

            // update the ticket
            itemsDetails.Add(ntd);
            TicketDetail.Items.Add(ntd);
            TicketDetail.Items.Refresh();

            totalPrice = 0;

            foreach (clsTicketDetail id in itemsDetails)
            {
                totalPrice += id.TotalPrice;
            }

            if (Settings.Default.ATVApplyFee)
            {
                totalIVAFee = totalPrice * 13 / 100;
                totalPrice += totalIVAFee;
            }

            lblTotalSale.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

            Discount.IsEnabled = true;
            Payment.IsEnabled = true;

            lBox_Products.UnselectAll();
            txtQtyProduct.Text = string.Empty;
            txtQtyProduct.IsEnabled = false;
            txtSearchProduct.Text = string.Empty;
            AddProduct.IsEnabled = false;

            mw.QuickSale.IsEnabled = false;

            if (Settings.Default.UltraQuickSale)
            {
                Cash.IsEnabled = true;
                CreditCard.IsEnabled = true;
                Transfer.IsEnabled = true;
            }
        }
        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            CleanAll();
        }
        private void btn_Usual(object sender, RoutedEventArgs e)
        {
            List<clsCustFreqItem> custFreqItemsList = DB.GetCustomerFrequentItems(0);

            if (custFreqItemsList.Count > 0)
            {
                this.Opacity = 0.5;
                wpfCustFreqItems cfi = new wpfCustFreqItems(custFreqItemsList);
                cfi.ShowDialog();
                this.Opacity = 1;

                custFreqItem = cfi.custFreqItem;

                if (custFreqItem == null) return;

                txtSearchProduct.Text = custFreqItem.ItemDescription;
            }
        }
        private void btn_PrintMeal(object sender, RoutedEventArgs e)
        {
            bool haveMeals = false;

            List<clsTicketDetail> itemMeal = new List<clsTicketDetail>();

            foreach (clsTicketDetail itemdg in TicketDetail.Items)
            {
                if (DB.IsMealItemType(itemdg.ItemDesc))
                {
                    clsTicketDetail ntd = new clsTicketDetail();

                    ntd.ItemDesc = itemdg.ItemDesc;
                    ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                    ntd.Qty = itemdg.Qty;
                    ntd.Note = itemdg.Note;

                    itemMeal.Add(ntd);
                    haveMeals = true;
                }
            }

            if (haveMeals)
                Helper.GetMealItemsFromTicket(Settings.Default.QuickOrderCustID, itemMeal);

            MealOrderReminder = false;
        }
        private void btn_Discount(object sender, RoutedEventArgs e)
        {
            try
            {
                // select payment method
                wpfEnterAmount amt = new wpfEnterAmount();
                amt.ShowDialog();

                if (amt.amount == 0) return; // CANCEL


                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = "DESCUENTO (-)";
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.Qty = 1;
                ntd.UnitCost = 0;
                ntd.TotalCost = 0;
                ntd.UnitPrice = amt.amount * -1;
                ntd.TotalPrice = amt.amount * -1;

                totalPrice += ntd.TotalPrice;

                lblTotalSale.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

                // update the ticket
                itemsDetails.Add(ntd);
                TicketDetail.Items.Add(ntd);
                TicketDetail.Items.Refresh();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void btn_Payment(object sender, RoutedEventArgs e)
        {
            if (MealOrderReminder)
            {
                wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: INCLUYÓ COMIDA EN LA ORDEN, PERO NO HA GENERADO LA COMANDA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            wpfPayMethod2 payForm = new wpfPayMethod2("-sp", totalPrice, 9001, true, 0);

            payForm.ShowDialog();

            if (payForm.payOK == false) return; // CANCEL

            foreach (clsTicketDetail itemdg in TicketDetail.Items)
            {
                clsItem item = new clsItem();
                item.ID = itemdg.ItemID;
                item.ItemSold = itemdg.Qty;
                DB.UpdateItemInventory("SAL", item);

                if (DB.IsMealItemType(itemdg.ItemDesc))
                {
                    Helper.ApplySaleToInvenytory(itemdg.ItemID, itemdg.Qty);
                }

                if (itemdg.Bucket)
                {
                    string[] bucketContent = itemdg.Note.Split('$');

                    foreach (string idg in bucketContent)
                    {
                        clsItem ni = new clsItem();
                        ni.ID = Convert.ToInt32(idg.Split(',')[0]);
                        ni.ItemSold = Convert.ToInt32(idg.Split(',')[1]);
                        DB.UpdateItemInventory("SAL", ni);
                    }
                }

                if (itemdg.ItemSubType == 4)    // PROMO
                {
                    clsPromoConfig promo = DB.GetPromotion(itemdg.ItemID);
                    clsItem ni = new clsItem();
                    ni.ID = promo.ItemID;
                    ni.ItemSold = promo.PromoQty * itemdg.Qty;
                    DB.UpdateItemInventory("SAL", ni);
                }
            }

            Guid guidID = Guid.NewGuid();

            int ticketNumber = DB.CreateNextTicket(guidID.ToString(), Settings.Default.QuickOrderCustID);

            DB.UpdateTicketStatus(ticketNumber, 0, 0, 0, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher,
                                  Settings.Default.WhoOpen, DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID));

            DB.InsertTicketDetail(itemsDetails, guidID.ToString(), Settings.Default.WhoOpen, true);

            // print cancelled ticket
            if (PrintClosedTicket.IsChecked == true)
            {
                ticket.ID = ticketNumber;
                ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
                ticket.CustomerID = DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID);
                ticket.CustomerAKA = ticket.CustomerID;
                ticket.Cash = payForm.cash;
                ticket.CreditCard = payForm.creditCard;
                ticket.Transfer = payForm.transfer;
                ticket.Status = false;
                ticket.IVAFee = totalIVAFee;
                Helper.PrintTicket(ticket);
            }

            if (payForm.transfer > 0)
            {
                if (Settings.Default.PrintSINPETicket)
                {
                    ticket.ID = ticketNumber;
                    ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
                    ticket.CustomerID = DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID);
                    ticket.Transfer = payForm.transfer;
                    Helper.PrintTicket(ticket, 1);
                }
            }

            if (Settings.Default.GenerateXMLforTicket)
            {
                Helper.GenerateXMLforTicket(ticketNumber);
            }

            if (Settings.Default.ATVApplyFee)
            {
                if (payForm.send2IRSforElectronicTicket || payForm.send2IRSforElectronicInvoice)
                {
                    ElectronicDoc ATV = new ElectronicDoc();
                    ATV.DocElectronico = new DocElectronico();

                    // header
                    ATV.DocElectronico.Token = Settings.Default.ATVToken;
                    ATV.DocElectronico.CodigoActividad = Settings.Default.ATVActivityCode;
                    ATV.DocElectronico.Cliente = Settings.Default.ATVClientCode;

                    if (payForm.send2IRSforElectronicInvoice)
                    {
                        mw.Opacity = 0.5;
                        wpfElectronicInvoice einv = new wpfElectronicInvoice(ticket.ID);
                        einv.ShowDialog();
                        mw.Opacity = 1;

                        if (einv.bCancel)
                        {
                            mw.transInProgress = false;
                            mw.transInProgressTries = 0;
                            return;
                        }

                        ATVQuery atvqry = new ATVQuery();

                        atvqry.TicketID = ticket.ID;
                        atvqry.CustomerName = einv.custName;
                        atvqry.SSN_Type = einv.custIDType;
                        atvqry.SSN = einv.custID;
                        atvqry.CountryCode = einv.custCountryCode;
                        atvqry.PhoneNumber = einv.custPhoneNumber;
                        atvqry.eMailAddress = einv.custEmail;

                        DB.InsertATVTicket(atvqry);

                        // receptor info
                        ATV.DocElectronico.Receptor = new WhoReceive();
                        ATV.DocElectronico.Receptor.Nombre = einv.custName;

                        ATV.DocElectronico.Receptor.Identificacion = new SSN();
                        ATV.DocElectronico.Receptor.Identificacion.Tipo = einv.custIDType;
                        ATV.DocElectronico.Receptor.Identificacion.Numero = einv.custID;

                        ATV.DocElectronico.Receptor.Telefono = new PhoneNumber();
                        ATV.DocElectronico.Receptor.Telefono.CodigoPais = einv.custCountryCode;
                        ATV.DocElectronico.Receptor.Telefono.NumTelefono = einv.custPhoneNumber;
                        ATV.DocElectronico.Receptor.CorreoElectronico = einv.custEmail;
                    }

                    // ticket header
                    ATV.DocElectronico.CondicionVenta = 1;

                    if (payForm.cash > 0 && payForm.creditCard == 0 && payForm.transfer == 0)
                    {
                        ATV.DocElectronico.MedioPago = "01";
                    }
                    else if (payForm.cash == 0 && payForm.creditCard > 0 && payForm.transfer == 0)
                    {
                        ATV.DocElectronico.MedioPago = "02";
                    }
                    else if (payForm.cash == 0 && payForm.creditCard == 0 && payForm.transfer > 0)
                    {
                        ATV.DocElectronico.MedioPago = "04";
                    }
                    else
                    {
                        if (payForm.cash > 0 && payForm.creditCard > 0 && payForm.transfer == 0)
                        {
                            ATV.DocElectronico.MedioPago = "01,02";
                        }
                        else if (payForm.cash > 0 && payForm.creditCard == 0 && payForm.transfer > 0)
                        {
                            ATV.DocElectronico.MedioPago = "01,04";
                        }
                        else if (payForm.cash > 0 && payForm.creditCard > 0 && payForm.transfer > 0)
                        {
                            ATV.DocElectronico.MedioPago = "01,02,04";
                        }
                        else if (payForm.cash == 0 && payForm.creditCard > 0 && payForm.transfer > 0)
                        {
                            ATV.DocElectronico.MedioPago = "02,04";
                        }
                        else
                        {
                            ATV.DocElectronico.MedioPago = "01";
                        }
                    }

                    // ticket detail
                    LineDetail lineDetail = new LineDetail();
                    lineDetail.NumeroLinea = 1;
                    lineDetail.Codigo = 6331000000000;

                    lineDetail.CodigoComercial = new ComercialCode();
                    lineDetail.CodigoComercial.Tipo = 1;
                    lineDetail.CodigoComercial.Codigo = 4;

                    lineDetail.Cantidad = 1;
                    lineDetail.UnidadMedida = "Unid";
                    lineDetail.Detalle = "SERVICIO DE RESTAURANTE";
                    lineDetail.PrecioUnitario = ticket.TotalPrice;

                    lineDetail.Descuento = new Discount();
                    lineDetail.Descuento.MontoDescuento = 0;
                    lineDetail.Descuento.NaturalezaDescuento = "SIN DESCUENTO";

                    lineDetail.SubTotal = ticket.TotalPrice;

                    lineDetail.Impuesto = new Tax();
                    lineDetail.Impuesto.Codigo = 1;
                    lineDetail.Impuesto.CodigoTarifa = 8;
                    lineDetail.Impuesto.Tarifa = 13;
                    lineDetail.Impuesto.Monto = lineDetail.SubTotal * 13 / 100;

                    lineDetail.MontoTotalLinea = lineDetail.SubTotal + lineDetail.Impuesto.Monto;

                    // ticket summary
                    ATV.DocElectronico.DetalleServicio = new ServiceDetail();
                    ATV.DocElectronico.DetalleServicio.LineaDetalle = new List<LineDetail>();
                    ATV.DocElectronico.DetalleServicio.LineaDetalle.Add(lineDetail);

                    ATV.DocElectronico.OtrosCargos = new OtherCharges();
                    ATV.DocElectronico.OtrosCargos.TipoDocumento = 6;
                    ATV.DocElectronico.OtrosCargos.Detalle = "Impuesto de Servicio 10%";
                    ATV.DocElectronico.OtrosCargos.MontoCargo = 0;

                    ATV.DocElectronico.ResumenFactura = new TicketSummary();
                    ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda = new CurrencyTypeCode();
                    ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.CodigoMoneda = "CRC";
                    ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.TipoCambio = 1;

                    // Serializing JSON
                    string jsonOutput = JsonConvert.SerializeObject(ATV);
                    JSON.ATVSendWebServiceCall(ticket.ID, jsonOutput);
                }
            }

            if (Settings.Default.UseCashDrawer)
            {
                xPrinterOpenCashbox xpCash = new xPrinterOpenCashbox();
                xpCash.print();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Open Cash Drawer request by user {Settings.Default.WhoOpen}", Logger.Severity.WARNING);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
            sw.ShowDialog();

            CleanAll();
        }
        private void btn_Increase(object sender, MouseButtonEventArgs e)
        {
            foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
            {
                rdi.Qty++;
                rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;
                totalPrice += rdi.UnitPrice;

                if (DB.IsMealItemType(rdi.ItemDesc))
                {
                    this.Opacity = 0.5;
                    wpfMealNote mn = new wpfMealNote(rdi.ItemDesc);
                    mn.ShowDialog();
                    this.Opacity = 1;
                    rdi.Note = mn.mealNote;

                    clsTicketDetail newMealOrder = new clsTicketDetail();

                    newMealOrder.Qty = 1;
                    newMealOrder.ItemID = rdi.ItemID;
                    newMealOrder.ItemDesc = rdi.ItemDesc;
                    newMealOrder.Note = rdi.Note;

                    newMealsOrder.Add(newMealOrder);
                }
                else
                {
                    clsTicketDetail newBeverageOrder = new clsTicketDetail();

                    newBeverageOrder.Qty = 1;
                    newBeverageOrder.ItemID = rdi.ItemID;
                    newBeverageOrder.ItemDesc = rdi.ItemDesc;
                    newBeverageOrder.Note = rdi.Note;
                    newBeverageOrder.Bucket = false;

                    if (DB.GetItemSubtype(rdi.ItemDesc) == 2)
                    {
                        this.Opacity = 0.5;
                        wpfSelectBucketContent mn = new wpfSelectBucketContent(newBeverageOrder.ItemID);
                        mn.ShowDialog();
                        this.Opacity = 1;
                        newBeverageOrder.Note = mn.bucketContent;
                        newBeverageOrder.Bucket = true;
                    }
                    newBeveragesOrder.Add(newBeverageOrder);
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ITEM {rdi.ItemID} INCREASED BY 1.", Logger.Severity.INFORMATION);
            }

            lblTotalSale.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

            TicketDetail.Items.Refresh();
        }
        private void btn_Delete(object sender, MouseButtonEventArgs e)
        {
            clsTicketDetail item = TicketDetail.SelectedItem as clsTicketDetail;

            totalPrice -= item.TotalPrice;
            lblTotalSale.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

            itemsDetails.RemoveAll(x => x.ItemID == item.ItemID);
            TicketDetail.Items.Remove(item);
            TicketDetail.Items.Refresh();

            Increase.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;

            if (Settings.Default.UltraQuickSale && TicketDetail.Items.Count == 0)
            {
                Cash.IsEnabled = false;
                CreditCard.IsEnabled = false;
                Transfer.IsEnabled = false;
            }
        }
        private void btn_Decrease(object sender, MouseButtonEventArgs e)
        {
            foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
            {
                rdi.Qty--;
                rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;
                totalPrice -= rdi.UnitPrice;

                if (DB.IsMealItemType(rdi.ItemDesc))
                {
                    try
                    {
                        var itemToRemove = newMealsOrder.First(r => r.ItemID == rdi.ItemID);
                        newMealsOrder.Remove(itemToRemove);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        var itemToRemove = newBeveragesOrder.First(r => r.ItemID == rdi.ItemID);
                        newBeveragesOrder.Remove(itemToRemove);
                    }
                    catch { }
                }
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ITEM {rdi.ItemID} DECREASED BY 1.", Logger.Severity.INFORMATION);
            }

            lblTotalSale.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

            TicketDetail.Items.Refresh();
        }
        private void btn_Cash(object sender, RoutedEventArgs e)
        {
            UltraQuickSale(1);
        }
        private void btn_CreditCard(object sender, RoutedEventArgs e)
        {
            UltraQuickSale(2);
        }
        private void btn_Tranfer(object sender, RoutedEventArgs e)
        {
            UltraQuickSale(3);
        }
        #endregion

    }
}
