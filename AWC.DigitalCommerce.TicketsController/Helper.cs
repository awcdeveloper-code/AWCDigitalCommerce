using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Controls;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace AWC.DigitalCommerce.TicketsController
{
    public class Helper
    {
        #region GLOBAL VARIABLES
        public static clsUser userProfile = new clsUser();
        public static clsTicket Ticket = new clsTicket();
        public static string CustomerID = string.Empty;
        public static SpeechSynthesizer _speechSynthesizer;
        #endregion

        #region GENERAL
        public static bool ApplySaleToInvenytory(int itemID, int qty)
        {
            try
            {
                List<clsItem> itemsList = DB.GetMealRelationships(itemID);
                
                foreach (clsItem item in itemsList)
                {
                    item.ItemSold = qty * Convert.ToInt32(item.ItemAvailable);
                    DB.UpdateItemInventory("SAL", item);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
        }
        public static int GetCurrencyExchange()
        {
            try
            {
                if (SMTP.CheckInternetConnection())
                {
                    string today = DB.ConverTicketDate(Settings.Default.BusinessDate).Replace(".", "/");

                    cr.fi.bccr.gee.wsindicadoreseconomicos bccrWS = new cr.fi.bccr.gee.wsindicadoreseconomicos();

                    DataSet ds = bccrWS.ObtenerIndicadoresEconomicos("317", today, today, "Guillermo Grillo", "N", "guillermoegrillo@outlook.com", "EOZMLUIEGI");

                    return Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[2].ToString().Split(',')[0]);
                }
                else
                {
                    return Settings.Default.USDollarExchangeRate;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return Settings.Default.USDollarExchangeRate;
            }
        }
        public static List<clsTicketsForDataGrid> ConvertTicketDetail2TicketDataGrid(List<clsTicketDetail> itemsList)
        {
            try
            {
                List<clsTicketsForDataGrid> itemsConverted = new List<clsTicketsForDataGrid>();

                foreach (clsTicketDetail itd in itemsList)
                {
                    clsTicketsForDataGrid ic = new clsTicketsForDataGrid();

                    

                    itemsConverted.Add(ic);
                }

                return itemsConverted;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> SortTicketsForDataGrid(List<clsTicketsForDataGrid> itemsList)
        {
            try
            {
                List<clsTicketsForDataGrid> itemsListSorted = new List<clsTicketsForDataGrid>();

                DataSet TempDB = new DataSet("TempDB");

                DataTable Tickets = TempDB.Tables.Add("Tickets");

                Tickets.Columns.Add("TicketID", typeof(int));
                Tickets.Columns.Add("CustomerName", typeof(string));
                Tickets.Columns.Add("TotalPrice", typeof(int));
                Tickets.Columns.Add("PayMethodAlpha", typeof(string));
                Tickets.Columns.Add("StatusAlpha", typeof(string));

                foreach (clsTicketsForDataGrid item in itemsList)
                {
                    Tickets.Rows.Add(item.ID, item.CustomerID, item.TotalPrice, item.PayMethodAlpha, item.StatusAlpha);
                }

                DataRow[] sortedRows;
                sortedRows = Tickets.Select("TicketID > 0", "TicketID ASC");

                foreach (DataRow row in sortedRows)
                {
                    clsTicketsForDataGrid itemSorted = new clsTicketsForDataGrid();

                    itemSorted.ID = Convert.ToInt32(row["TicketID"]);
                    itemSorted.CustomerID = row["CustomerName"].ToString();
                    itemSorted.TotalPrice = Convert.ToInt32(row["TotalPrice"]);
                    itemSorted.PayMethodAlpha = row["PayMethodAlpha"].ToString();
                    itemSorted.StatusAlpha = row["StatusAlpha"].ToString();

                    itemsListSorted.Add(itemSorted);
                }

                return itemsListSorted;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static bool CheckLicenseExpiration()
        {
            bool licenseExpired = true;

            string licensePath = Path.Combine(Directory.GetCurrentDirectory(), "AWC.DigitalCommerce.TicketsController.Lic");

            if (!File.Exists(licensePath))
            {
                wpfMessageBox.Show("Tickets Controller",
                                   "ERROR: No se ha encontrado la licencia de uso para TICKETS CONTROLLER. Por favor, comuníquese con AIDAware Consultancies inmediatamente.",
                                   System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return false;
            }

            using (StreamReader sr = new StreamReader(licensePath))
            {
                string licenseEncoded = sr.ReadToEnd();

                byte[] data = Convert.FromBase64String(licenseEncoded);

                string decodedString = Encoding.UTF8.GetString(data);

                string licenseDecoded = decodedString.Split(';')[4];

                if (Convert.ToInt32(licenseDecoded.Split('=')[1]) < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")))
                    licenseExpired = true;
                else
                if (Convert.ToInt32(licenseDecoded.Split('=')[1]) - Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")) <= 7)
                {
                    int remainingDays = Convert.ToInt32(licenseDecoded.Split('=')[1]) - Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                    wpfMessageBox.Show("Tickets Controller",
                                        $"ATENCIÓN: Su licencia de uso TICKETS CONTROLLER expirará en tan sólo {remainingDays} días. Por favor, comuníquese con AIDAware Consultancies para la respectiva renovación.",
                                        System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    return false;
                }
                else
                if (Convert.ToInt32(licenseDecoded.Split('=')[1]) >= Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")))
                    licenseExpired = false;
            }

            return licenseExpired;
        }
        public static void ShareTicketAndCustomerID(clsTicket _ticket, string customerId)
        {
            Ticket = _ticket;
            CustomerID = customerId;
        }
        public static void CheckDateOfWeekForBackup()
        {
            DateTime dt = DateTime.Now;

            if (dt.DayOfWeek == (DayOfWeek)Settings.Default.DatabaseBackupDayOfWeek)
            {
                if (!Settings.Default.WeeklyBackupSent)
                {
                    foreach (string fname in Directory.GetFiles(Settings.Default.DatabaseBackupLocation, "*.bak"))
                    {
                        FileInfo info = new FileInfo(fname);
                        DateTime createDatetime = info.CreationTime;
                        TimeSpan diff = DateTime.Now - createDatetime;

                        if (diff.Days > Settings.Default.DatabaseBackupExpiration)
                        {
                            File.Delete(fname);
                            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"{fname}: {createDatetime} expired.", Logger.Severity.WARNING);
                        }
                    }

                    // create ZIP file for BAK file
                    //string bakFullFileName = DB.AWCDigitalCommerceDBBackup();
                    //string bakWithoutSuffix = Path.GetFileNameWithoutExtension(bakFullFileName);
                    //string backZipFileName = bakWithoutSuffix + ".zip";

                    //using (FileStream zipFile = File.Open(backZipFileName, FileMode.Create))
                    //{
                    //    // File to be added to archive
                    //    using (FileStream source = File.Open(DB.AWCDigitalCommerceDBBackup(), FileMode.Open, FileAccess.Read))
                    //    {
                    //        using (var archive = new Archive(new ArchiveEntrySettings()))
                    //        {
                    //            archive.CreateEntry(backZipFileName, source);
                    //            archive.Save(zipFile);
                    //        }
                    //    }
                    //}

                    SMTP.SendAWCDigitalCommerceBackup(DB.AWCDigitalCommerceDBBackup());

                    Settings.Default.WeeklyBackupSent = true;
                    Settings.Default.Save();

                    if (Settings.Default.PowerUser > 0)
                    {
                        DB.TurnOffPowerUserTickets(Settings.Default.PowerUser);
                    }

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Full database backup and Log directory clean made successfully.", Logger.Severity.INFORMATION);
                }
            }
            else
            {
                Settings.Default.WeeklyBackupSent = false;
                Settings.Default.Save();
            }
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "CheckDateOfWeekForBackup validation PASSED.", Logger.Severity.DEBUG);
        }
        public static void GetMealItemsFromTicket(int customerID, List<clsTicketDetail> itemsDetails)
        {
            if (Settings.Default.OldTicketsDate.Length > 0) return;

            List<string> mealList = new List<string>();
            string beverageList = string.Empty;

            string custName = DB.GetCustomerIDByID(customerID);
            int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, customerID);

            bool haveMeal = false;
            bool haveBeverages = false;
            int isBucket = 0;

            foreach (clsTicketDetail itDet in itemsDetails)
            {
                if (DB.IsMealItemType(itDet.ItemDesc))
                {
                    mealList.Add(itDet.Qty.ToString() + "|" + itDet.ItemDesc + "|" + itDet.Note);
                    haveMeal = true;
                }
                else
                {
                    isBucket = itDet.Bucket == true ? 1 : 0;
                    beverageList += isBucket.ToString() + "|" + itDet.Qty.ToString() + "|" + itDet.ItemDesc + "|" + itDet.Note + "^";
                    haveBeverages = true;
                }
            }

            if (haveMeal)
            {
                Helper.PrintTicket(custName, mealList, true);
            }

            if (haveBeverages)
            {
                DB.InsertBartenderOrder(ticketNum.ToString() + "^" + custName + "|" + Settings.Default.WhoOpen.ToString(), beverageList);
            }
        }
        public static void GetMealItemsFromTicket(string empName, List<clsTicketDetail> itemsDetails)
        {
            List<string> mealList = new List<string>();

            foreach (clsTicketDetail itDet in itemsDetails)
                mealList.Add(itDet.Qty.ToString() + "|" + itDet.ItemDesc + "|" + itDet.Note);

            Helper.PrintTicket(empName, mealList, true);
        }
        public static void GetBeverageItemsFromTicket(string customerID, List<clsTicketDetail> itemsDetails)
        {
            try
            {
                string beveragesList = string.Empty;
                int isBucket = 0;

                foreach (clsTicketDetail itDet in itemsDetails)
                {
                    isBucket = itDet.Bucket == true ? 1 : 0;
                    beveragesList += isBucket + "|" + itDet.Qty.ToString() + "|" + itDet.ItemDesc + "|" + itDet.Note + "^";
                }

                DB.InsertBartenderOrder(customerID + "|" + Settings.Default.WhoOpen.ToString(), beveragesList);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        public static string RevertFormatDate(string dt)
        {
            return dt.Substring(6, 4) + dt.Substring(3, 2) + dt.Substring(0, 2);
        }
        public static string RandomString(int length)
        {
            Random rnd = new Random();
            const string pool = "0123456789abcdefghijklmnopqrstuvwxyz";
            var chars = Enumerable.Range(0, length).Select(x => pool[rnd.Next(0, pool.Length)]);
            return new string(chars.ToArray()).ToUpper();
        }
        public static clsUser CheckUserProfile(string PIN)
        {
            try
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "CheckUserProfile VALIDATION passed", Logger.Severity.DEBUG);
                return userProfile = DB.CheckUserPIN(PIN);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }

        }
        public static bool CheckUserAccessToResource(string objName)
        {
            try
            {
                bool status = false;

                switch (objName)
                {
                    case "NewTicket":
                        status = userProfile.userSecurityProfile.Substring(0, 1) == "1" ? true : false;
                        break;
                    case "NewCustomer":
                        status = userProfile.userSecurityProfile.Substring(1, 1) == "1" ? true : false;
                        break;
                    case "UpdateTicket":
                        status = userProfile.userSecurityProfile.Substring(2, 1) == "1" ? true : false;
                        break;
                    case "CloseTicket":
                        status = userProfile.userSecurityProfile.Substring(3, 1) == "1" ? true : false;
                        break;
                    case "RemoveItem":
                        status = userProfile.userSecurityProfile.Substring(4, 1) == "1" ? true : false;
                        break;
                    case "SplitItem":
                        status = userProfile.userSecurityProfile.Substring(5, 1) == "1" ? true : false;
                        break;
                    case "ToApply":
                        status = userProfile.userSecurityProfile.Substring(6, 1) == "1" ? true : false;
                        break;
                    case "CancelTicket":
                        status = userProfile.userSecurityProfile.Substring(7, 1) == "1" ? true : false;
                        break;
                    case "OpenTickets":
                        status = userProfile.userSecurityProfile.Substring(8, 1) == "1" ? true : false;
                        break;
                    case "TodaySales":
                        status = userProfile.userSecurityProfile.Substring(9, 1) == "1" ? true : false;
                        break;
                    case "Queries":
                        status = userProfile.userSecurityProfile.Substring(10, 1) == "1" ? true : false;
                        break;
                    case "ChangePIN":
                        status = userProfile.userSecurityProfile.Substring(11, 1) == "1" ? true : false;
                        break;
                    case "Maintenance":
                        status = userProfile.userSecurityProfile.Substring(12, 1) == "1" ? true : false;
                        break;
                    case "InventoryMgmt":
                        status = userProfile.userSecurityProfile.Substring(13, 1) == "1" ? true : false;
                        break;
                }
                return status;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"CheckUserAccessToResource for [{userProfile.userName}]: {userProfile.userSecurityProfile}", Logger.Severity.ERROR);
                return false;
            }
        }
        public static bool CheckUserAccessToResource2(string objName)
        {
            try
            {
                bool status = false;

                switch (objName)
                {
                    case "QuickSale":
                        status = userProfile.userSecurityProfile.Substring(0, 1) == "1" ? true : false;
                        break;
                    case "Tickets":
                        status = userProfile.userSecurityProfile.Substring(1, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_CreateTicket":
                        status = userProfile.userSecurityProfile.Substring(2, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_PrintTicket":
                        status = userProfile.userSecurityProfile.Substring(3, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_AbortTicket":
                        status = userProfile.userSecurityProfile.Substring(4, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_CancelUpdate":
                        status = userProfile.userSecurityProfile.Substring(5, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_UpdateTicket":
                        status = userProfile.userSecurityProfile.Substring(6, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_SmallPayment":
                        status = userProfile.userSecurityProfile.Substring(7, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_PayTicket":
                        status = userProfile.userSecurityProfile.Substring(8, 1) == "1" ? true : false;
                        break;
                    case "Queries":
                        status = userProfile.userSecurityProfile.Substring(10, 1) == "1" ? true : false;
                        break;
                    case "Inventory":
                        status = userProfile.userSecurityProfile.Substring(11, 1) == "1" ? true : false;
                        break;
                    case "OldTickets":
                        status = userProfile.userSecurityProfile.Substring(12, 1) == "1" ? true : false;
                        break;
                    case "ucOldTickets_PrintTicket":
                        status = userProfile.userSecurityProfile.Substring(13, 1) == "1" ? true : false;
                        break;
                    case "ucOldTickets_AbortTicket":
                        status = userProfile.userSecurityProfile.Substring(14, 1) == "1" ? true : false;
                        break;
                    case "ucOldTickets_SmallPaymentTicket":
                        status = userProfile.userSecurityProfile.Substring(15, 1) == "1" ? true : false;
                        break;
                    case "ucOldTickets_PayTicket":
                        status = userProfile.userSecurityProfile.Substring(16, 1) == "1" ? true : false;
                        break;
                    case "TodaySales":
                        status = userProfile.userSecurityProfile.Substring(17, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_Print":
                        status = userProfile.userSecurityProfile.Substring(18, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_PrintClosed":
                        status = userProfile.userSecurityProfile.Substring(19, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_PrintFoodService":
                        status = userProfile.userSecurityProfile.Substring(20, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_AbortTicket":
                        status = userProfile.userSecurityProfile.Substring(21, 1) == "1" ? true : false;
                        break;
                    case "DailyClose":
                        status = userProfile.userSecurityProfile.Substring(22, 1) == "1" ? true : false;
                        break;
                    case "Maintenance":
                        status = userProfile.userSecurityProfile.Substring(23, 1) == "1" ? true : false;
                        break;
                    case "ucOldTickets_ReassignTicket":
                        status = userProfile.userSecurityProfile.Substring(24, 1) == "1" ? true : false;
                        break;
                    case "CashDrawer":
                        status = userProfile.userSecurityProfile.Substring(27, 1) == "1" ? true : false;
                        break;
                    case "ucTickets_AddOldTicket":
                        status = userProfile.userSecurityProfile.Substring(32, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_FakeTicket":
                        status = userProfile.userSecurityProfile.Substring(33, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_eMailTicket":
                        status = userProfile.userSecurityProfile.Substring(34, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_Daily":
                        status = userProfile.userSecurityProfile.Substring(35, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_GralExpenses":
                        status = userProfile.userSecurityProfile.Substring(36, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_DefectiveItems":
                        status = userProfile.userSecurityProfile.Substring(37, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_UsersMgmt":
                        status = userProfile.userSecurityProfile.Substring(38, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_Categories":
                        status = userProfile.userSecurityProfile.Substring(39, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_LoyaltyMgmt":
                        status = userProfile.userSecurityProfile.Substring(40, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_TicketsMgmt":
                        status = userProfile.userSecurityProfile.Substring(41, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_ChangeName":
                        status = userProfile.userSecurityProfile.Substring(43, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_ElectronicInvoice":
                        status = userProfile.userSecurityProfile.Substring(44, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_IncomeCash":
                        status = userProfile.userSecurityProfile.Substring(45, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_InternalOrders":
                        status = userProfile.userSecurityProfile.Substring(46, 1) == "1" ? true : false;
                        break;
                    case "Maintenance_Specials":
                        status = userProfile.userSecurityProfile.Substring(47, 1) == "1" ? true : false;
                        break;
                    case "ucTodaySales_ChangePayMethod":
                        status = userProfile.userSecurityProfile.Substring(48, 1) == "1" ? true : false;
                        break;
                }
                return status;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false;
            }
        }
        public static Visibility CheckUserAccessToResource3(string objName)
        {
            try
            {
                Visibility status = Visibility.Hidden;

                switch (objName)
                {
                    case "ucTickets_btnAdd":
                        status = userProfile.userSecurityProfile.Substring(28, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                        break;
                    case "ucTickets_btnDel":
                        status = userProfile.userSecurityProfile.Substring(29, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                        break;
                    case "ucTickets_btnSub":
                        status = userProfile.userSecurityProfile.Substring(30, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                        break;
                    case "ucTickets_btnPrn":
                        status = userProfile.userSecurityProfile.Substring(31, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                        break;
                }
                return status;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return Visibility.Hidden;
            }
        }
        public static void ShowMessage(string msg, MessageBoxIcon icon)
        {
            System.Windows.Forms.MessageBox.Show(msg, "Tickets Controller", MessageBoxButtons.OK, icon);
        }
        private static void OpenPrintedTicket(string fileName)
        {
            Process firstProc = new Process();
            firstProc.StartInfo.FileName = "notepad.exe";
            firstProc.StartInfo.Arguments = fileName;
            firstProc.EnableRaisingEvents = true;

            firstProc.Start();
            firstProc.WaitForExit();
        }
        public static void AddServiceFee(int totalPrice, List<clsItemDetailForDatagrid> itemdg, ucCloseTicket addServiceFee)
        {
            try
            {
                clsItemDetailForDatagrid serviceFee = new clsItemDetailForDatagrid();
                serviceFee.ID = 55555;
                serviceFee.ItemDesc = "10% SERVICIO";
                serviceFee.Qty = 1;
                serviceFee.UnitPrice = serviceFee.UnitPrice = totalPrice * 10 / 100;
                serviceFee.TotalPrice = serviceFee.UnitPrice;
                itemdg.Add(serviceFee);

                addServiceFee.TicketDetail.ItemsSource = itemdg;
                addServiceFee.TicketDetail.Items.Refresh();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static void AddServiceFee(int totalPrice, List<clsItemDetailForDatagrid> itemdg, wpfFastTrack addServiceFee)
        {
            try
            {
                clsItemDetailForDatagrid serviceFee = new clsItemDetailForDatagrid();
                serviceFee.ID = 55555;
                serviceFee.ItemDesc = "10% SERVICIO";
                serviceFee.Qty = 1;
                serviceFee.UnitPrice = serviceFee.UnitPrice = totalPrice * 10 / 100;
                serviceFee.TotalPrice = serviceFee.UnitPrice;
                itemdg.Add(serviceFee);

                //addServiceFee.TicketDetail.ItemsSource = itemdg;
                addServiceFee.TicketDetail.Items.Refresh();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static void AddServiceFee(int totalPrice, List<clsItemDetailForDatagrid> itemdg, wpfQryTicketByNumber addServiceFee)
        {
            try
            {
                clsItemDetailForDatagrid serviceFee = new clsItemDetailForDatagrid();
                serviceFee.ItemID = 55555;
                serviceFee.ItemDesc = "10% SERVICIO";
                serviceFee.Qty = 1;
                serviceFee.UnitPrice = totalPrice;
                serviceFee.TotalPrice = totalPrice;
                itemdg.Add(serviceFee);

                addServiceFee.TicketDetail.ItemsSource = itemdg;
                addServiceFee.TicketDetail.Items.Refresh();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static clsTicketsForDataGrid Convert2TicketsForDataGrid(clsTicket ticket, string custName)
        {
            try
            {
                clsTicketsForDataGrid id4dg = new clsTicketsForDataGrid();

                id4dg.TicketDate = DB.ConverTicketDate(ticket.TicketDate);
                id4dg.ID = ticket.ID;
                id4dg.CustomerID = custName;
                id4dg.ServiceFee = ticket.ServiceFee;
                id4dg.IVAFee = ticket.IVAFee;
                id4dg.TotalPrice = ticket.TotalPrice;
                id4dg.Cash = ticket.Cash;
                id4dg.CreditCard = ticket.CreditCard;
                id4dg.Transfer = ticket.Transfer;
                id4dg.PayMethod = ticket.PayMethod;
                id4dg.Status = ticket.Status;

                return id4dg;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public static bool GenerateXMLforTicket(int ticketNum)
        {
            try
            {
                string xmlStoragePath = Path.Combine(Settings.Default.SerilogRootPath, "XMLStorage");

                xmlStoragePath = Path.Combine(xmlStoragePath, $"{DateTime.Now.ToString("yyyy-MM")}");

                if (!Directory.Exists(xmlStoragePath))
                    Directory.CreateDirectory(xmlStoragePath);

                // serialize Ticket Header
                string xmlTicket = Path.Combine(xmlStoragePath, ticketNum.ToString("000000") + "H.xml");

                clsTicket ticket = DB.GetTicket(ticketNum);

                XmlSerializer serialTicket = new XmlSerializer(typeof(clsTicket));

                using (StringWriter textWriter = new StringWriter())
                {
                    serialTicket.Serialize(textWriter, ticket);

                    using (StreamWriter sw = new StreamWriter(xmlTicket))
                    {
                        sw.WriteLine(textWriter.ToString());
                    }
                }

                // serialize Ticket Detaik
                string xmlTicketDetail = Path.Combine(xmlStoragePath, ticketNum.ToString("000000") + "D.xml");

                List<clsItemDetailForDatagrid> ticketDetails = DB.GetItemsByGUID(ticket.GUID, false);

                XmlSerializer serialTicketDetail = new XmlSerializer(typeof(List<clsItemDetailForDatagrid>));

                using (StringWriter textWriter = new StringWriter())
                {
                    serialTicketDetail.Serialize(textWriter, ticketDetails);

                    using (StreamWriter sw = new StreamWriter(xmlTicketDetail))
                    {
                        sw.WriteLine(textWriter.ToString());
                    }
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Ticket {ticketNum.ToString("000000")} was serialized successfully.", Logger.Severity.ERROR);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false;
            }
        }
        public static bool ValidateInternalAccounts()
        {
            if (!DB.CheckInternalAccount(Settings.Default.DeletedID))
            {
                wpfMessageBox.Show("Tickets Controller",
                                   "ERROR: Registro 'DeletedID' no existe en la base de datos, la aplicación será abortada. Por favor, comuníquese con AIDAware Consultancies inmediatamente.",
                                   System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return false;
            }

            if (!DB.CheckInternalAccount(Settings.Default.QuickOrderCustID))
            {
                wpfMessageBox.Show("Tickets Controller",
                                   "ERROR: Registro 'QuickOrderCustID' no existe en la base de datos, la aplicación será abortada. Por favor, comuníquese con AIDAware Consultancies inmediatamente.",
                                   System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return false;
            }

            if (!DB.CheckInternalAccount(Settings.Default.SplitTicketCustID))
            {
                wpfMessageBox.Show("Tickets Controller",
                                   "ERROR: Registro 'SplitTicketCustID' no existe en la base de datos, la aplicación será abortada. Por favor, comuníquese con AIDAware Consultancies inmediatamente.",
                                   System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return false;
            }

            return true;
        }
        public static void InDevelopment()
        {
            wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: Esta opción no está disponible en este momento.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
        }
        public static bool CharsInText(string text, int length)
        {
            try
            {
                int charCounter = 0;
                string validChar = "QWERTYUIOPASDFGHJKLZXCVBNMÑ1234567890";
                string c = string.Empty;

                text = text.ToUpper();

                for (int i = 1; i <= text.Length; i++)
                {
                    c = text.Substring(i - 1, 1);

                    if (validChar.Contains(c))
                    {
                        charCounter++;
                    }
                }

                if (charCounter >= length)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static void CleanTempFiles(string tempDir, int fileAgeInDays)
        {
            if (string.IsNullOrEmpty(tempDir))
            {
                throw new ArgumentException("Temporary directory path cannot be null or empty.", nameof(tempDir));
            }

            if (!Directory.Exists(tempDir))
            {
                throw new DirectoryNotFoundException($"The specified directory does not exist: {tempDir}");
            }

            try
            {
                var directoryInfo = new DirectoryInfo(tempDir);
                var now = DateTime.Now;

                foreach (var file in directoryInfo.GetFiles())
                {
                    var fileAge = now - file.CreationTime;

                    if (fileAge.TotalDays > fileAgeInDays)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch { }
                    }
                }

                foreach (var subDir in directoryInfo.GetDirectories())
                {
                    var dirAge = now - subDir.CreationTime;

                    if (dirAge.TotalDays > fileAgeInDays)
                    {
                        try
                        {
                            subDir.Delete(true);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error cleaning temporary files: {ex.Message}", "Tickets Controller", MessageBoxButtons.OK);
            }
        }
        public static void ShowToastNotification(string message, int duration = 3000)
        {
            wpfToastNotification toast = new wpfToastNotification(message.ToUpper(), duration);
            toast.Left = SystemParameters.WorkArea.Right - toast.Width;
            toast.Top = SystemParameters.WorkArea.Bottom - toast.Height;
            toast.Show();
        }
        public static async Task<string> GetDailyQuote()
        {
            // https://zenquotes.io/api/today

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(Settings.Default.GetDailyQuote);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JArray json = JArray.Parse(responseBody);

                return $"{json[0]["q"].ToString()}." + Environment.NewLine + $"{json[0]["a"].ToString()}";
            }
            catch (Exception e)
            {
                return $"ERROR:{e.Message}";
            }
        }
        #endregion

        #region PRINT TICKET
        public static void PrintTicketWithXPrinterOrig()
        {
            xPrinterTicketOrig xPrintTck = new xPrinterTicketOrig();
            xPrintTck.destination = "USB001";
            xPrintTck.TicketNo = 03022022;
            xPrintTck.amount = 99000;
            xPrintTck.ticketDate = DateTime.Now;
            xPrintTck.source = "Tickets Controller";
            xPrintTck.drawnBy = "GGrillo";
            xPrintTck.print();
        }

        public static void PrintTicket(string customerID, List<clsTicketDetail> itemsDetails)
        {
            try
            {
                xPrinterOrder xPrintOrd = new xPrinterOrder(customerID, itemsDetails);
                xPrintOrd.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(clsTicket ticket, List<clsTicketDetail> itemsDetails, string customerName)
        {
            try
            {
                xPrintSplitedItems xPrintOrd = new xPrintSplitedItems(ticket, itemsDetails, customerName);
                xPrintOrd.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(List<clsSalesHistory> salesHist, string dates)
        {
            try
            {
                xPrinterSalesHistory xPrintSalesHist = new xPrinterSalesHistory(salesHist, dates);
                xPrintSalesHist.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(clsTicketsForDataGrid ticket)
        {
            try
            {
                if (Settings.Default.PrintTicketRemotely)
                {
                    string xmlString = ConvertToXML(ticket);
                    DB.InsertTicketToPrintRemotely(xmlString);
                }
                else
                {
                    xPrinterTicket xPrintTck = new xPrinterTicket(ticket);
                    xPrintTck.print();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(clsTicketsForDataGrid ticket, int option, string newName = "")
        {
            try
            {
                switch (option)
                {
                    case 0: // regular
                        xPrinterTicket xPrintTck = new xPrinterTicket(ticket, newName);
                        xPrintTck.print();
                        break;
                    case 1: // SINPE Voucher
                        xPrintSINPEVoucher xPrintSINPEVoucher = new xPrintSINPEVoucher(ticket);
                        xPrintSINPEVoucher.print();
                        break;
                    case 2: // FOOD SERVICE Voucher
                        xPrintFoodServiceVoucher xPrintFoodServiceVoucher = new xPrintFoodServiceVoucher(ticket, newName);
                        xPrintFoodServiceVoucher.print();
                        break;
                    case 3: // GENERATE BMP FOR EMAIL
                        xPrinterTicketForEMail xPrinterTicketForEMail = new xPrinterTicketForEMail(ticket);
                        xPrinterTicketForEMail.print();
                        break;
                    case 4: // GENERATE BMP VOUCHER FOR EMAIL
                        xPrinterVoucherForEMail xPrinterVoucherForEMail = new xPrinterVoucherForEMail(ticket);
                        xPrinterVoucherForEMail.print();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(List<clsItem> itemsPriceList)
        {
            try
            {
                xPrinterItemsPriceList xPrintIPL = new xPrinterItemsPriceList(itemsPriceList);
                xPrintIPL.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string custName, List<string> mealList, bool IsMeal)
        {
            try
            {
                if (IsMeal)
                {
                    xPrinterMealOrder xPrintMealTck = new xPrinterMealOrder(custName, mealList);
                    xPrintMealTck.print();
                }
                else
                {
                    xPrinterBeveragesOrder xPrintBeveragesTck = new xPrinterBeveragesOrder(custName, mealList);
                    xPrintBeveragesTck.print();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string workDay, List<clsTicketsForDataGrid> ticketsList, int action)
        {
            try
            {
                switch(action)
                {
                    case 0:
                        xPrinterDailyClose xPrintDayClose = new xPrinterDailyClose(workDay, ticketsList);
                        xPrintDayClose.print();
                        break;
                    case 1:
                        xPrinterTicketsPerCustomer xPrintTcksPerCust = new xPrinterTicketsPerCustomer(workDay, ticketsList);
                        xPrintTcksPerCust.print();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(List<clsItemDetailForDatagrid> itemList, int opt, string startDate, string finishDate)
        {
            try
            {
                switch(opt)
                {
                    case 1:
                        xPrinterMealsSummary xPrintMealSumm = new xPrinterMealsSummary(itemList);
                        xPrintMealSumm.print();
                        break;
                    case 2:
                        xPrinterConsumption xPrintConsum = new xPrinterConsumption(itemList, startDate, finishDate);
                        xPrintConsum.print();
                        break;
                    case 3:
                        xPrinterBeveragesSummary xPrintBeveragesSumm = new xPrinterBeveragesSummary(itemList);
                        xPrintBeveragesSumm.print();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string cmd, List<clsItem> itemsPriceList)
        {
            try
            {
                switch (cmd)
                {
                    case "BelowMinimum":
                        xPrinterItemsBelowMinimum xPrintMinimum = new xPrinterItemsBelowMinimum(itemsPriceList);
                        xPrintMinimum.print();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(List<clsDelincuency> delincuenciesList)
        {
            try
            {
                xPrinterDelincuenciesList xPrintDL = new xPrinterDelincuenciesList(delincuenciesList);
                xPrintDL.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(clsSmallPayment smlPay)
        {
            try
            {
                xPrinterSmallPayment xPrintSmallPay = new xPrinterSmallPayment(smlPay);
                xPrintSmallPay.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string workDay, string totServiceFee)
        {
            try
            {
                xPrintServiceFeeVoucher xPrintServiceFee = new xPrintServiceFeeVoucher(workDay, totServiceFee);
                xPrintServiceFee.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string workDay, clsDailyClosing dc)
        {
            try
            {
                xPrinterDailyCloseSummary xPrintDCSummary = new xPrinterDailyCloseSummary(workDay, dc);
                xPrintDCSummary.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string workDay1, string workDay2, clsDailyClosing dc)
        {
            try
            {
                xPrinterDailyCloseSummary xPrintDCSummary = new xPrinterDailyCloseSummary(workDay1, workDay2, dc);
                xPrintDCSummary.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintTicket(string workDay1, string workDay2, List<clsServiceFeeByWho> sfbwl)
        {
            try
            {
                xPrintServiceFeeByWho xPrintSFBWSummary = new xPrintServiceFeeByWho(workDay1, workDay2, sfbwl);
                xPrintSFBWSummary.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintInventory(List<clsItem> itemsList)
        {
            try
            {
                xPrinterItemsInventory xPrintInventory = new xPrinterItemsInventory(itemsList);
                xPrintInventory.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintInventoryByParts(List<clsItem> itemsList, string type)
        {
            try
            {
                xPrinterItemsInventoryByParts xPrintInventoryByParts = new xPrinterItemsInventoryByParts(itemsList, type);
                xPrintInventoryByParts.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintInvoicesSummaryByDate(List<clsItemDetailForDatagrid> newItemsByDate, string startDate, string endDate)
        {
            try
            {
                xPrinterInvoicesSummaryByDate xPrintInventory = new xPrinterInvoicesSummaryByDate(newItemsByDate, startDate, endDate);
                xPrintInventory.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintBusinessCard()
        {
            try
            {
                xPrintBusinessCard xPrintBC = new xPrintBusinessCard();
                xPrintBC.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void PrintInternalOrder(string fileName)
        {
            try
            {
                xPrintInternalOrder xPrintIO = new xPrintInternalOrder(fileName);
                xPrintIO.print();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FORMAT LINE
        public static string FormatCustlLine(string str2print)
        {
            return new string('>', 12 - (str2print.Length / 2)) + str2print + new string('<', 12 - (str2print.Length / 2));
        }

        public static string FormatGralLine(string str2print)
        {
            return new string(' ', 15 - (str2print.Length / 2)) + str2print;
        }

        public static string FormatGralLine(string str2print, int lenght)
        {
            return str2print + new string(' ', lenght - str2print.Length);
        }

        public static string FormatItemDetailLine(clsItemDetailForDatagrid item2print)
        {
            if (item2print.ItemDesc.Length > 19)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 19);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 19 - item2print.ItemDesc.Length);

            string qty = item2print.Qty.ToString();
            string tot = item2print.TotalPrice.ToString("N0");

            string line = qty.PadLeft(2) + " " +
                          item2print.ItemDesc + " " +
                          tot.PadLeft(7);
            return line;
        }

        public static string FormatSplitItemDetailLine(clsTicketDetail item2print)
        {
            if (item2print.ItemDesc.Length > 19)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 19);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 19 - item2print.ItemDesc.Length);

            string qty = item2print.Qty.ToString();
            string tot = item2print.TotalPrice.ToString("N0");

            string line = qty.PadLeft(2) + " " +
                          item2print.ItemDesc + " " +
                          tot.PadLeft(7);
            return line;
        }

        public static string FormatItemDetailLine(clsTicketDetail item2print)
        {
            string qty = item2print.Qty.ToString();
            string line = qty.PadLeft(2) + " " + item2print.ItemDesc;
            return line;
        }

        public static string FormatItemDetailLineSummary(clsItemDetailForDatagrid item2print)
        {
            if (item2print.ItemDesc.Length > 18)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 18);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 18 - item2print.ItemDesc.Length);

            string qty = item2print.Qty.ToString();
            string tot = item2print.TotalPrice.ToString("N0");

            string line = qty.PadLeft(3) + " " +
                          item2print.ItemDesc + " " +
                          tot.PadLeft(7);
            return line;
        }

        public static string FormatMealItemDetailLineSummary(clsItemDetailForDatagrid item2print)
        {
            if (item2print.ItemDesc.Length > 18)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 18);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 18 - item2print.ItemDesc.Length);

            string qty = item2print.Qty.ToString();
            string tot = item2print.TotalCost.ToString("N0");

            string line = qty.PadLeft(3) + " " +
                          item2print.ItemDesc + " " +
                          tot.PadLeft(7);
            return line;
        }

        public static string FormatConsumptionItemDetailLineSummary(clsItemDetailForDatagrid item2print)
        {
            if (item2print.ItemDesc.Length > 18)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 18);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 18 - item2print.ItemDesc.Length);

            string qty = item2print.Qty.ToString();
            string tot = item2print.TotalPrice.ToString("N0");

            string line = qty.PadLeft(3) + " " +
                          item2print.ItemDesc + " " +
                          tot.PadLeft(7);
            return line;
        }

        public static string FormatItemPriceLine(clsItem item2print)
        {
            if (item2print.ItemDescription.Length > 22)
                item2print.ItemDescription = item2print.ItemDescription.Substring(0, 22);
            else
                item2print.ItemDescription = item2print.ItemDescription + new string(' ', 22 - item2print.ItemDescription.Length);

            string uPrice = item2print.UnitPrice.ToString("N0");

            string line = item2print.ItemDescription + " " + uPrice.PadLeft(7);
            return line;
        }

        public static string FormatItemDetailLine(int Qty, string mealDesc)
        {
            return Qty.ToString() + " " + mealDesc;

            //if (mealDesc.Length > 20)
            //    mealDesc = mealDesc.Substring(0, 20);
            //else
            //    mealDesc = mealDesc + new string(' ', 20 - mealDesc.Length);

            //return Qty.ToString().PadLeft(2) + " " + mealDesc;
        }

        public static string FormatMinimumItemLine(clsItem item)
        {
            if (item.ItemDescription.Length > 17)
                item.ItemDescription = item.ItemDescription.Substring(0, 17);
            else
                item.ItemDescription = item.ItemDescription + new string(' ', 17 - item.ItemDescription.Length);

            string min = item.ItemMinimum.ToString("N0");
            string ava = item.ItemAvailable.ToString("N0");

            string line = item.ItemDescription + "   " + min.PadLeft(4) + "  " + ava.PadLeft(4);
            return line;
        }

        public static string FormatExpenseLine(clsExpense exp)
        {
            if (exp.ExpenseDescription.Length > 22)
                exp.ExpenseDescription = exp.ExpenseDescription.Substring(0, 22);
            else
                exp.ExpenseDescription = exp.ExpenseDescription + new string(' ', 22 - exp.ExpenseDescription.Length);

            string tot = exp.ExpenseAmount.ToString("N0");

            string line = exp.ExpenseDescription + " " + tot.PadLeft(7);
            return line;
        }

        public static string FormatSmallPaymentLine(string custName, int ticketID)
        {
            if (custName.Length > 22)
                custName = custName.Substring(0, 22);
            else
                custName = custName + new string('-', 22 - custName.Length);

            string line = custName + "  " + ticketID.ToString("000000");
            return line;
        }

        public static string FormatLunchLine(string lunchLine)
        {
            string qty = lunchLine.Split('|')[0];
            string itemDesc = lunchLine.Split('|')[1];
            int total = Convert.ToInt32(lunchLine.Split('|')[2]);

            if (itemDesc.Length > 20)
                itemDesc = itemDesc.Substring(0, 20);
            else
                itemDesc = itemDesc + new string(' ', 20 - itemDesc.Length);

            string tot = total.ToString("N0");

            string line = qty.PadLeft(2) + " " +
                          itemDesc + " " +
                          tot.PadLeft(6);
            return line;
        }
        public static string FormatInventoryLine(clsItem item2print)
        {
            if (item2print.ItemDescription.Length > 24)
                item2print.ItemDescription = item2print.ItemDescription.Substring(0, 24);
            else
                item2print.ItemDescription = item2print.ItemDescription + new string(' ', 24 - item2print.ItemDescription.Length);

            string uAvail = item2print.ItemAvailable.ToString("N0");
            string line = item2print.ItemDescription + uAvail.PadLeft(6);
            return line;
        }
        public static string FormatInvoiceSummaryLine(clsItemDetailForDatagrid item2print)
        {
            if (item2print.ItemDesc.Length > 24)
                item2print.ItemDesc = item2print.ItemDesc.Substring(0, 24);
            else
                item2print.ItemDesc = item2print.ItemDesc + new string(' ', 24 - item2print.ItemDesc.Length);

            string uAvail = item2print.Qty.ToString("N0");
            string line = item2print.ItemDesc + uAvail.PadLeft(6);
            return line;
        }

        #endregion

        #region XML Transformation
        public static string ConvertToXML(clsTicketsForDataGrid ticket)
        {
            try
            {
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(ticket.GetType());
                    serializer.Serialize(stringwriter, ticket);
                    return stringwriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        public static clsTicketsForDataGrid LoadFromXMLString(string xmlText)
        {
            try
            {
                using (var stringReader = new System.IO.StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(clsTicketsForDataGrid));
                    return serializer.Deserialize(stringReader) as clsTicketsForDataGrid;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region TEXT-TO-SPEECH
        public static void TextToSpeech(string textToSpeak)
        {
            try
            {
                _speechSynthesizer = new SpeechSynthesizer();
                _speechSynthesizer.Rate = 0;
                _speechSynthesizer.Volume = 100;

                if (!string.IsNullOrEmpty(textToSpeak))
                {
                    _speechSynthesizer.Speak(textToSpeak);
                    _speechSynthesizer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("TextToSpeech", ex.Message, Logger.Severity.ERROR);
            }
        }
        #endregion

        #region ZIP FILES
        public static string ZIPDatabase(string fullPath)
        {
            try
            {
                string zipPath = Path.GetDirectoryName(fullPath);
                string fileName = Path.Combine(zipPath, Path.GetFileNameWithoutExtension(fullPath));
                string zipFilePath = fileName + ".zip";
                string fn = Path.GetFileName(fullPath);

                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(fullPath, fn);
                    }
                }

                File.Delete(fullPath);

                return zipFilePath;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, null);
                return string.Empty;
            }
        }
        #endregion
    }
}
