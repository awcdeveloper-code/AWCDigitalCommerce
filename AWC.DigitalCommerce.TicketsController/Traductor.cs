using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Web.Script.Serialization;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Controls;

namespace AWC.DigitalCommerce.TicketsController
{
    public class Traductor
    {
        public static void ApplyTranslation(wpfFastTrack wpfFT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfFastTrack"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfFastTrack"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // BUTTONS
                wpfFT.CreateTicket.Content = form.Properties["CreateTicket"];
                wpfFT.PrintTicket.Content = form.Properties["PrintTicket"];
                wpfFT.AbortTicket.Content = form.Properties["AbortTicket"];
                wpfFT.CancelUpdate.Content = form.Properties["CancelUpdate"];
                wpfFT.UpdateTicket.Content = form.Properties["UpdateTicket"];
                wpfFT.SmallPayment.Content = form.Properties["SmallPayment"];
                wpfFT.PayTicket.Content = form.Properties["PayTicket"];
                wpfFT.CloseFastTrack.Content = form.Properties["CloseFastTrack"];
                wpfFT.Express.Content = form.Properties["Express"];
                wpfFT.SplitTicket.Content = form.Properties["SplitTicket"];

                // LABELS
                wpfFT.lblCustomersList.Content = form.Properties["lblCustomersList"];
                wpfFT.lblProductsList.Content = form.Properties["lblProductsList"];

                // MESSAGES
                wpfFT.strNoTicket = form.Properties["strNoTicket"];
                wpfFT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                wpfFT.strQtyEqualZero = form.Properties["strQtyEqualZero"];
                wpfFT.strNoRemoveMeal = form.Properties["strNoRemoveMeal"];
                wpfFT.strNoRemoveItem = form.Properties["strNoRemoveItem"];
                wpfFT.strRemoveItem = form.Properties["strRemoveItem"];
                wpfFT.strAbortTicket = form.Properties["strAbortTicket"];
                wpfFT.strPendingUpdate = form.Properties["strPendingUpdate"];
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucTickets ucTcks, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfFastTrack"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfFastTrack"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // BUTTONS
                ucTcks.CreateTicket.Content = form.Properties["CreateTicket"];
                ucTcks.PrintTicket.Content = form.Properties["PrintTicket"];
                ucTcks.AbortTicket.Content = form.Properties["AbortTicket"];
                ucTcks.CancelUpdate.Content = form.Properties["CancelUpdate"];
                ucTcks.UpdateTicket.Content = form.Properties["UpdateTicket"];
                ucTcks.SmallPayment.Content = form.Properties["SmallPayment"];
                ucTcks.PayTicket.Content = form.Properties["PayTicket"];
                ucTcks.SplitTicket.Content = form.Properties["SplitTicket"];

                // LABELS
                ucTcks.lblCustomersList.Content = form.Properties["lblCustomersList"];
                //ucTcks.lblProductsList.Content = form.Properties["lblProductsList"];

                // MESSAGES
                ucTcks.strNoTicket = form.Properties["strNoTicket"];
                ucTcks.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucTcks.strQtyEqualZero = form.Properties["strQtyEqualZero"];
                ucTcks.strNoRemoveMeal = form.Properties["strNoRemoveMeal"];
                ucTcks.strNoRemoveItem = form.Properties["strNoRemoveItem"];
                ucTcks.strRemoveItem = form.Properties["strRemoveItem"];
                ucTcks.strAbortTicket = form.Properties["strAbortTicket"];
                ucTcks.strPendingUpdate = form.Properties["strPendingUpdate"];
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfMainWindow wpfMW, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMainWindow"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMainWindow"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                wpfMW.WindowTitle.Text = form.WindowTitle;
                wpfMW.CopyRights.Content = form.Properties["CopyRights"];
                wpfMW.DailyTransactions.Header = form.Properties["DailyTransactions"];
                wpfMW.Miscellaneous.Header = form.Properties["Miscellaneous"];
                wpfMW.Administration.Header = form.Properties["Administration"];
                wpfMW.WorkArea.Header = form.Properties["WorkArea"];
                // BUTTONS
                wpfMW.NewTicket.Content = form.Properties["NewTicket"];
                wpfMW.UpdateTicket.Content = form.Properties["UpdateTicket"];
                wpfMW.CloseTicket.Content = form.Properties["CloseTicket"];
                wpfMW.OldTickets.Content = form.Properties["OldTickets"];
                wpfMW.TodaySalesReport.Content = form.Properties["TodaySalesReport"];
                wpfMW.Queries.Content = form.Properties["Queries"];
                wpfMW.SystemMaintenance.Content = form.Properties["SystemMaintenance"];
                wpfMW.InventoryManagement.Content = form.Properties["InventoryManagement"];
                wpfMW.SwitchPIN.Content = form.Properties["SwitchPIN"];
                wpfMW.Exit.Content = form.Properties["Exit"];
                // MESSAGES
                wpfMW.strLicenseExpired = form.Properties["strLicenseExpired"];
                wpfMW.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                wpfMW.strNoOpenTickets = form.Properties["strNoOpenTickets"];
                wpfMW.strMinimumAvailable = form.Properties["strMinimumAvailable"];
                wpfMW.strWelcomeAboard = form.Properties["strWelcomeAboard"];
                wpfMW.strBusinessDate = form.Properties["strBusinessDate"];
                wpfMW.strBusinessDateLog = form.Properties["strBusinessDateLog"];
                wpfMW.strBusinessDateLogAlert = form.Properties["strBusinessDateLogAlert"];
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfMainWindow2 wpfMW, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMainWindow"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMainWindow"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }
                // MESSAGES
                wpfMW.strLicenseExpired = form.Properties["strLicenseExpired"];
                wpfMW.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                wpfMW.strNoOpenTickets = form.Properties["strNoOpenTickets"];
                wpfMW.strMinimumAvailable = form.Properties["strMinimumAvailable"];
                wpfMW.strWelcomeAboard = form.Properties["strWelcomeAboard"];
                wpfMW.strBusinessDate = form.Properties["strBusinessDate"];
                wpfMW.strBusinessDateLog = form.Properties["strBusinessDateLog"];
                wpfMW.strBusinessDateLogAlert = form.Properties["strBusinessDateLogAlert"];
                wpfMW.strItemsBelowZero = form.Properties["strItemsBelowZero"];
                wpfMW.strInventoryOK = form.Properties["strInventoryOK"];
                wpfMW.strInternetOK = form.Properties["strInternetOK"];
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucCloseTicket ucCT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucCloseTicket"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucCloseTicket"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucCT.lblTotal.Content = form.Properties["lblTotal"];
                ucCT.GroupBoxDataGrid.Header = form.Properties["GroupBoxDataGrid"];
                ucCT.PrintSummary.Content = form.Properties["strPrintSummary"];
                ucCT.PrintClosedTicket.Content = form.Properties["PrintClosedTicket"];
                // BUTTONS
                ucCT.Clean.Content = form.Properties["Clean"];
                ucCT.CloseTicket.Content = form.Properties["CloseTicket"];
                ucCT.RemoveItem.Content = form.Properties["RemoveItem"];
                ucCT.SplitTicket.Content = form.Properties["SplitTicket"];
                // MESSAGES
                ucCT.strCustomerIDNotFound = form.Properties["strCustomerIDNotFound"];
                ucCT.strPrintTicket = form.Properties["strPrintTicket"];
                ucCT.strCloseTicket = form.Properties["strCloseTicket"];
                ucCT.strRemoveItem = form.Properties["strRemoveItem"];
                ucCT.strNoRemoveMeal = form.Properties["strNoRemoveMeal"];
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucExpensesReport ucER, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucExpensesReport"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucExpensesReport"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                ucER.grpGralExpenses.Header = form.Properties["grpGralExpenses"];
                ucER.lblDate.Content = form.Properties["lblDate"];
                ucER.lblDescription.Content = form.Properties["lblDescription"];
                ucER.lblAmount.Content = form.Properties["lblAmount"];
                ucER.Add.Content = form.Properties["Add"];
                ucER.strExpenseAdded = form.Properties["strExpenseAdded"];

                ucER.lblLunchDate.Content = form.Properties["lblDate"];
                ucER.lblName.Content = form.Properties["lblName"];
                ucER.lblLunch.Content = form.Properties["lblLunch"];
                ucER.lblQty.Content = form.Properties["lblQty"];
                ucER.AddLunch.Content = form.Properties["Add"];
                ucER.strLunchAdded = form.Properties["strLunchAdded"];

                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucNewTicket ucNT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucNT.VIPGroupBox.Header = form.Properties["VIPGroupBox"];
                ucNT.TablesSeatsGroupBox.Header = form.Properties["TablesSeatsGroupBox"];
                ucNT.NewCustomerGroupBox.Header = form.Properties["NewCustomerGroupBox"];
                ucNT.lblBeerSoda.Content = form.Properties["lblBeerSoda"];
                ucNT.lblLiqour.Content = form.Properties["lblLiqour"];
                ucNT.lblMeal.Content = form.Properties["lblMeal"];
                // BUTTONS
                ucNT.AddBeer.Content = form.Properties["Add"];
                ucNT.AddLiqour.Content = form.Properties["Add"];
                ucNT.AddMeal.Content = form.Properties["Add"];
                ucNT.Clean.Content = form.Properties["Clean"];
                ucNT.TakeOrder.Content = form.Properties["TakeOrder"];
                //// MESSAGES
                ucNT.strCustomerExist = form.Properties["strCustomerExist"];
                ucNT.strCustomerNoExist = form.Properties["strCustomerNoExist"];
                ucNT.strCustomerAdded = form.Properties["strCustomerAdded"];
                ucNT.strValueCannotBeZero = form.Properties["strValueCannotBeZero"];
                ucNT.strCustomerIDNotFound = form.Properties["strCustomerIDNotFound"];
                ucNT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucNT.strTickedAdded = form.Properties["strTickedAdded"];
                ucNT.strERRORsavingTckDet = form.Properties["strERRORsavingTckDet"];
                ucNT.strERRORsavingTck = form.Properties["strERRORsavingTck"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfNewTicketStep1 ucNT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucNT.VIPGroupBox.Header = form.Properties["VIPGroupBox"];
                ucNT.TablesSeatsGroupBox.Header = form.Properties["TablesSeatsGroupBox"];
                // BUTTONS
                ucNT.Close.Content = form.Properties["Close"];
                ucNT.Cancel.Content = form.Properties["Clean"];
                ucNT.CustomerSelected.Content = form.Properties["CustomerSelected"];
                // MESSAGES
                ucNT.strCustomerExist = form.Properties["strCustomerExist"];
                ucNT.strCustomerNoExist = form.Properties["strCustomerNoExist"];
                ucNT.strCustomerAdded = form.Properties["strCustomerAdded"];
                ucNT.strValueCannotBeZero = form.Properties["strValueCannotBeZero"];
                ucNT.strCustomerIDNotFound = form.Properties["strCustomerIDNotFound"];
                ucNT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucNT.strTickedAdded = form.Properties["strTickedAdded"];
                ucNT.strERRORsavingTckDet = form.Properties["strERRORsavingTckDet"];
                ucNT.strERRORsavingTck = form.Properties["strERRORsavingTck"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfNewTicket wpfNT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicket"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                //wpfNT.VIPGroupBox.Header = form.Properties["VIPGroupBox"];
                //wpfNT.TablesSeatsGroupBox.Header = form.Properties["TablesSeatsGroupBox"];
                //wpfNT.lblBeerSoda.Content = form.Properties["lblBeerSoda"];
                //wpfNT.lblLiqour.Content = form.Properties["lblLiqour"];
                //wpfNT.lblMeal.Content = form.Properties["lblMeal"];
                //// BUTTONS
                //wpfNT.AddBeer.Content = form.Properties["Add"];
                //wpfNT.AddLiqour.Content = form.Properties["Add"];
                //wpfNT.AddMeal.Content = form.Properties["Add"];
                //wpfNT.Clean.Content = form.Properties["Clean"];
                //wpfNT.TakeOrder.Content = form.Properties["TakeOrder"];
                // MESSAGES
                wpfNT.strCustomerExist = form.Properties["strCustomerExist"];
                wpfNT.strCustomerNoExist = form.Properties["strCustomerNoExist"];
                wpfNT.strCustomerAdded = form.Properties["strCustomerAdded"];
                wpfNT.strValueCannotBeZero = form.Properties["strValueCannotBeZero"];
                wpfNT.strCustomerIDNotFound = form.Properties["strCustomerIDNotFound"];
                wpfNT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                wpfNT.strTickedAdded = form.Properties["strTickedAdded"];
                wpfNT.strERRORsavingTckDet = form.Properties["strERRORsavingTckDet"];
                wpfNT.strERRORsavingTck = form.Properties["strERRORsavingTck"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucNewTicketDetail ucNT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicketDetail"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucNewTicketDetail"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucNT.GroupBoxDataGrid.Header = form.Properties["GroupBoxDataGrid"];
                // BUTTONS
                ucNT.Cancel.Content = form.Properties["Clean"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucOldTickets ucOT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucOldTickets"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucOldTickets"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucOT.grpDefaultCustomers.Header = form.Properties["grpDefaultCustomers"];
                ucOT.grpOpenTickets.Header = form.Properties["grpOpenTickets"];
                // BUTTONS
                //ucOT.PrintTicket.Content = form.Properties["PrintTicket"];
                //ucOT.PayTicket.Content = form.Properties["PayTicket"];
                // MESSAGES
                ucOT.strTotalAmount = form.Properties["strTotalAmount"];
                ucOT.strPayAccount = form.Properties["strPayAccount"];
                ucOT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucOT.sttNoOldTickets = form.Properties["sttNoOldTickets"];
                ucOT.strMultiPayMethodNotAllowed = form.Properties["strMultiPayMethodNotAllowed"];
                ucOT.strAbortTicket = form.Properties["strAbortTicket"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucQueries ucQry, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucQueries"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucQueries"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucQry.Tickets.Header = form.Properties["Tickets"];
                ucQry.Customers.Header = form.Properties["Customers"];
                ucQry.Miscellaneous.Header = form.Properties["Miscellaneous"];
                // BUTTONS
                ucQry.TicketDetail.Content = form.Properties["TicketDetail"];
                ucQry.Catalog.Content = form.Properties["Catalog"];
                ucQry.TicketsByCustomer.Content = form.Properties["TicketsByCustomer"];
                ucQry.PriceList.Content = form.Properties["PriceList"];
                ucQry.Consumptions.Content = form.Properties["Consumptions"];
                ucQry.Graphics.Content = form.Properties["Graphics"];
                //ucQry.Providers.Content = form.Properties["Providers"];
                ucQry.Inventory.Content = form.Properties["Inventory"];
                ucQry.ActivityLog.Content = form.Properties["ActivityLog"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucTodaySales ucTS, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTodaySales"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTodaySales"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucTS.GroupBoxDataGrid.Header = form.Properties["GroupBoxDataGrid"];
                // BUTTONS
                //ucTS.Print.Content = form.Properties["Print"];
                //ucTS.PrintClosed.Content = form.Properties["PrintClosed"];
                // MESSAGES
                ucTS.strPrintAllTickets = form.Properties["strPrintAllTickets"];
                ucTS.strPrintAllClosedTickets = form.Properties["strPrintAllClosedTickets"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucUpdateTicket ucUT, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucUpdateTicket"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucUpdateTicket"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS & GROUPBOXES
                ucUT.lblBeverages.Content = form.Properties["lblBeverages"];
                ucUT.lblLiqours.Content = form.Properties["lblLiqours"];
                ucUT.lblMeals.Content = form.Properties["lblMeals"];
                // BUTTONS
                ucUT.AddBeer.Content = form.Properties["btnADD"];
                ucUT.AddLiqour.Content = form.Properties["btnADD"];
                ucUT.AddMeal.Content = form.Properties["btnADD"];
                ucUT.CleanOrder.Content = form.Properties["btnCleanOrder"];
                ucUT.TakeOrder.Content = form.Properties["btnTakeOrder"];
                // MESSAGES
                ucUT.strValueCannotBeZero = form.Properties["strValueCannotBeZero"];
                ucUT.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucUT.strCustomerIDNotFound = form.Properties["strCustomerIDNotFound"];
                ucUT.strTickedUpdated = form.Properties["strTickedUpdated"];
                ucUT.strERRORsavingTck = form.Properties["strERRORsavingTck"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfPayMethod2 wpfPM2, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfPayMethod2"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfPayMethod2"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS
                wpfPM2.lblCash.Content = form.Properties["lblCash"];
                wpfPM2.lblCreditCard.Content = form.Properties["lblCreditCard"];
                wpfPM2.lblTransfer.Content = form.Properties["lblTransfer"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfMessageBox wpfMB, string lang)
        {
            try
            {
                if (lang?.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMessageBox"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfMessageBox"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // BUTTONS
                wpfMB.btnOk.Content = form.Properties["btnOk"];
                wpfMB.btnYes.Content = form.Properties["btnYes"];
                wpfMB.btnNo.Content = form.Properties["btnNo"];
                wpfMB.btnCancel.Content = form.Properties["btnCancel"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfSplashWindow wpfSW, string lang)
        {
            try
            {
                if (lang?.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfSplashWindow"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfSplashWindow"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS
                wpfSW.lblProcessing.Content = form.Properties["lblProcessing"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucTablesMaintenance ucTM, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTablesMaintenance"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTablesMaintenance"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                // LABELS
                ucTM.grpBoxCustomers.Header = form.Properties["grpBoxCustomers"];
                ucTM.lblCustomerName.Content = form.Properties["lblCustomerName"];
                ucTM.lblCustomerType.Content = form.Properties["lblCustomerType"];
                ucTM.strCustomerExist = form.Properties["strCustomerExist"];
                ucTM.strActionResult = form.Properties["strActionResult"];
                ucTM.strActionQuestion = form.Properties["strActionQuestion"];
                ucTM.strChildParentEquals = form.Properties["strChildParentEquals"];
                ucTM.strRelationAlreadyExist = form.Properties["strRelationAlreadyExist"];
                ucTM.strRelationCreated = form.Properties["strRelationCreated"];
                ucTM.strRelationFailed = form.Properties["strRelationFailed"];
                ucTM.strRelationDelete = form.Properties["strRelationDelete"];
                ucTM.strRelationDeleted = form.Properties["strRelationDeleted"];
                ucTM.strRelationDeleteFailed = form.Properties["strRelationDeleteFailed"];

                ucTM.grpBoxBevLiqMea.Header = form.Properties["grpBoxBevLiqMea"];
                ucTM.lblItemName.Content = form.Properties["lblItemName"];
                ucTM.lblItemPrice.Content = form.Properties["lblItemPrice"];
                ucTM.lblItemType.Content = form.Properties["lblItemType"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(ucTicketsMaintenance ucTK, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTicketsMaintenance"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("ucTicketsMaintenance"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                ucTK.grpBox_TicketsList.Header = form.Properties["grpBox_TicketsList"];
                ucTK.grpBox_MoveTicket.Header = form.Properties["grpBox_MoveTicket"];
                ucTK.grpBox_InheritTicket.Header = form.Properties["grpBox_InheritTicket"];
                ucTK.grpBox_CancelTicket.Header = form.Properties["grpBox_CancelTicket"];
                ucTK.btnReassignTicket.Content = form.Properties["btnReassignTicket"];
                ucTK.btnInheritTicket.Content = form.Properties["btnInheritTicket"];
                ucTK.btnCancel.Content = form.Properties["btnCancel"];
                ucTK.btnCancelTicket.Content = form.Properties["btnCancelTicket"];

                ucTK.strCancelTicket = form.Properties["strCancelTicket"];
                ucTK.strPINdoNotExist = form.Properties["strPINdoNotExist"];
                ucTK.strReassignTicket = form.Properties["strReassignTicket"];
                ucTK.strTicketReassigned = form.Properties["strTicketReassigned"];
                ucTK.strInheritTicket = form.Properties["strInheritTicket"];
                ucTK.strTicketInherited = form.Properties["strTicketInherited"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void ApplyTranslation(wpfCashRegisterOpen wpfCRO, string lang)
        {
            try
            {
                if (lang.Length == 0) return;    // no translation is required

                // read dictionary
                string transDict = File.ReadAllText(Settings.Default.TranslationDictionary);

                // remove special chars
                transDict = string.Join(string.Empty, transDict.Split(new char[] { '\r', '\n', '\t', '\b' }));

                // deserialize and classify the content
                JavaScriptSerializer oJSS = new JavaScriptSerializer();
                clsConfig cnfg = oJSS.Deserialize<clsConfig>(transDict);

                clsLanguage language = new clsLanguage();
                clswpfForm form = new clswpfForm();

                switch (lang)
                {
                    case "-en":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_en"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfCashRegisterOpen"));
                        break;
                    case "-sp":
                        // locate translations
                        language = cnfg.Languages.Find(l => l.Name.Equals("lang_sp"));
                        form = language.Forms.Find(p => p.Name.Equals("wpfCashRegisterOpen"));
                        break;
                    default:
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Argument invalid, no translation applied.", Logger.Severity.WARNING);
                        return;
                }

                #region ASSIGN TRANSLATION
                wpfCRO.lblCashRegisterOpening.Content = form.Properties["lblCashRegisterOpening"];
                wpfCRO.btnContinue.Content = form.Properties["btnContinue"];
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
