using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;
using iText.StyledXmlParser.Jsoup.Select;
using MaterialDesignThemes.Wpf;
using Microsoft.Office.Core;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Utilities.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AWC.DigitalCommerce.TicketsController
{
    public class DB
    {
        #region Global Variables
        public static SqlConnection sqlConn;
        public static SqlCommand sqlCmd = null;
        public static SqlTransaction sqlTrans = null;
        public static bool filterON = true;
        #endregion

        #region SQL CONNECTION
        public static SqlConnection Open(string connStr)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(connStr);
                sqlConn.Open();
                return sqlConn;
            }
            catch (Exception ex)
            {
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static void Close(SqlConnection sqlConn)
        {
            try
            {
                if (sqlConn != null)
                    sqlConn.Close();
            }
            catch (Exception ex)
            {
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BINDINGS
        public static DataSet DataBinding_tbl_CustomerID(int custType, int status, string customerID = null)
        {
            try
            {
                string sqlQuery = string.Empty;

                switch (custType)
                {
                    case 0: // All
                        sqlQuery = "SELECT CustomerID FROM tbl_CustomerID WHERE Type = 1";
                        break;
                    case 1: // VIP
                    case 2: // Tables and Seats
                        sqlQuery = "SELECT ID, CustomerID FROM tbl_CustomerID WHERE Type = " + custType + " AND Active = " + status;
                        break;
                    case 3: // Just Active, no matter what type
                        sqlQuery = "SELECT ID, CustomerID FROM tbl_CustomerID WHERE Type <> 3 AND Active = " + status;
                        break;
                    case 4: // Just Active, no matter what type but no include customerID
                        sqlQuery = "SELECT ID, CustomerID FROM tbl_CustomerID WHERE CustomerID <> '" + customerID + "' AND Type <> 3 AND Active = " + status;
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet dtSet = new DataSet();

                    adapter.SelectCommand = sqlCmd;
                    adapter.Fill(dtSet, "tbl_CustomerID");
                    return dtSet;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static DataSet DataBinding_tbl_Items(int itemType)
        {
            try
            {
                string sqlQuery = "SELECT ID, ItemDescription FROM tbl_Items WHERE IsActive = 1 AND ItemType = " + itemType;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet dtSet = new DataSet();

                    adapter.SelectCommand = sqlCmd;
                    adapter.Fill(dtSet, "tbl_Items");
                    return dtSet;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsCustomerVIP> ListBinding_tbl_CustomerID(int custType, int status)
        {
            try
            {
                List<clsCustomerVIP> lstCustVIP = new List<clsCustomerVIP>();

                string sqlQuery = string.Empty;

                switch (custType)
                {
                    case 0: // All
                        sqlQuery = "SELECT * FROM tbl_CustomerID WHERE Type <> 3";
                        break;
                    case 1: // VIP
                    case 2: // Tables and Seats
                        sqlQuery = $"SELECT * FROM tbl_CustomerID WHERE Type = {custType} AND Active = {status}";
                        break;
                    case 3: // Just Active, no matter what type
                        sqlQuery = $"SELECT * FROM tbl_CustomerID WHERE Type <> 3 AND Active = {status}";
                        break;
                    case 4: // Just VIP, no matter status
                        sqlQuery = "SELECT * FROM tbl_CustomerID WHERE Type = 1";
                        break;
                    case 5: // VIP. Tables and Seats Available
                        sqlQuery = $"SELECT * FROM tbl_CustomerID WHERE Type IN (1,2) AND Active = {status}";
                        break;
                    case 6: // Just Tables and Seats
                        sqlQuery = "SELECT * FROM tbl_CustomerID WHERE Type = 2";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCustomerVIP custVIP = new clsCustomerVIP();
                            custVIP.ID = Convert.ToInt32(sdr["ID"]);
                            custVIP.Type = Convert.ToInt32(sdr["Type"]);
                            custVIP.CustomerID = sdr["CustomerID"].ToString();
                            custVIP.Active = Convert.ToBoolean(sdr["Active"]);
                            custVIP.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                            custVIP.LastPayment = ConverTicketDate(sdr["LastPayment"].ToString());
                            custVIP.CustomerFOC = Convert.ToBoolean(sdr["FreeOfCharge"]);
                            custVIP.CreditLimit = Convert.ToInt32(sdr["CreditLimit"]);
                            lstCustVIP.Add(custVIP);
                        }
                    }
                }
                return lstCustVIP;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsCustomerVIP> ListBinding_tbl_OpenTickets()
        {
            try
            {
                List<clsCustomerVIP> lstCustVIP = new List<clsCustomerVIP>();

                string sqlQuery = "SELECT * FROM tbl_OpenTickets";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCustomerVIP custVIP = new clsCustomerVIP();
                            custVIP.ID = Convert.ToInt32(sdr["ID"]);
                            custVIP.Type = Convert.ToInt32(sdr["Type"]);
                            custVIP.CustomerID = sdr["CustomerID"].ToString();
                            custVIP.Active = Convert.ToBoolean(sdr["Active"]);
                            custVIP.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                            custVIP.LastPayment = ConverTicketDate(sdr["LastPayment"].ToString());
                            custVIP.CustomerFOC = Convert.ToBoolean(sdr["FreeOfCharge"]);

                            if (custVIP.CustomerFOC)
                            {
                                custVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\NoPayment.png";

                            }
                            else
                            {
                                switch (custVIP.Type)
                                {
                                    case 1:
                                        custVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\icons8-tarjeta-de-membresia-94.png";
                                        break;
                                    case 2:
                                        custVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\tables.png";
                                        break;
                                    case 3:
                                        custVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\damage.png";
                                        break;
                                }
                            }

                            lstCustVIP.Add(custVIP);
                        }
                    }
                }
                return lstCustVIP;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> ListBinding_tbl_Items(int itemType)
        {
            try
            {
                List<clsItem> lstItems = new List<clsItem>();

                string sqlQuery = string.Empty;

                switch(itemType)
                {
                    case 0:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE ItemType <> 9 AND ItemSubType <> 3 ORDER BY ItemDescription ASC";
                        break;
                    case 1:
                        sqlQuery = $"SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemSubType <> 3 AND ItemType = {itemType}";
                        break;
                    case 2:
                        sqlQuery = $"SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemSubType <> 3 AND ItemType = {itemType} AND ID NOT IN (SELECT DISTINCT ItemParent FROM tbl_Items where ItemParent <> 0)";
                        break;
                    case 3:
                        sqlQuery = $"SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemSubType <> 3 AND ItemType = {itemType}";
                        break;
                    case 4:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE IsActive = 1 AND (ItemType = 1 OR ItemType = 2) AND ItemSubType <> 3 ORDER BY ItemDescription ASC";
                        break;
                    case 5:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE IsActive = 1 AND  ItemType <> 9 AND ItemSubType <> 3 AND ID NOT IN (SELECT DISTINCT ItemParent FROM tbl_Items where ItemParent <> 0) ORDER BY ItemDescription ASC";
                        break;
                    case 6:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemType <> 9 AND ItemSubType <> 3 AND ID NOT IN (SELECT DISTINCT ItemParent FROM tbl_Items where ItemParent <> 0) ORDER BY ItemDescription ASC";
                        break;
                    case 7:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE ItemType <> 9 ORDER BY ItemDescription ASC";
                        break;
                    case 8:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE ItemSubType = 3 ORDER BY ItemDescription ASC";
                        break;
                    case 9:
                        sqlQuery = $"SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemSubType <> 3 AND ItemType = {itemType}";
                        break;
                    case 10:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE ItemType <> 9 ORDER BY ItemDescription ASC";
                        break;
                    default:
                        return null;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();

                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.ItemType = Convert.ToInt32(sdr["ItemType"]);
                            item.ItemSubType = Convert.ToInt32(sdr["ItemSubType"]);
                            item.ItemDescription = sdr["ItemDescription"].ToString();
                            item.IsActive = Convert.ToBoolean(sdr["IsActive"]);
                            item.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            item.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                            item.ItemAvailable = Convert.ToInt32(sdr["ItemAvailable"]);
                            item.ItemSold = Convert.ToInt32(sdr["ItemSold"]);
                            item.ItemDefective = Convert.ToInt32(sdr["ItemDefective"]);
                            item.DebitNotes = Convert.ToInt32(sdr["DebitNotes"]);
                            item.CreditNotes = Convert.ToInt32(sdr["CreditNotes"]);
                            item.ItemParent = Convert.ToInt32(sdr["ItemParent"]);
                            item.ItemMinimum = Convert.ToInt32(sdr["ItemMinimum"]);
                            item.ItemStock = Convert.ToInt32(sdr["ItemStock"]);
                            item.ItemUnitOfMeasurement = Convert.ToInt32(sdr["ItemUnitOfMeasurement"]);
                            item.ItemUnitSize = Convert.ToInt32(sdr["ItemUnitSize"]);

                            switch (item.ItemType)
                            {
                                case 0:
                                    item.ImagePath = @"C:\AWC.DigitalCommerce\Images\NoAvailable.png";
                                    break;
                                case 1:
                                    item.ImagePath = @"C:\AWC.DigitalCommerce\Images\beer.png";
                                    break;
                                case 2:
                                    item.ImagePath = @"C:\AWC.DigitalCommerce\Images\liquors.ico";
                                    break;
                                case 3:
                                    item.ImagePath = @"C:\AWC.DigitalCommerce\Images\kitchen.ico";
                                    break;
                                case 9:
                                    item.ImagePath = @"C:\AWC.DigitalCommerce\Images\otherTrans.png";
                                    break;
                            }

                            switch (item.ItemSubType)
                            {
                                case 0:
                                    item.ImagePath2 = @"C:\AWC.DigitalCommerce\Images\zero.png";
                                    break;
                                case 2:
                                    item.ImagePath2 = @"C:\AWC.DigitalCommerce\Images\beer-bottle.png";
                                    break;
                                case 4:
                                    item.ImagePath2 = @"C:\AWC.DigitalCommerce\Images\p-64.png";
                                    break;
                            }

                            switch (item.IsActive)
                            {
                                case false:
                                    item.ImagePath3 = @"C:\AWC.DigitalCommerce\Images\20.ico";
                                    break;
                                case true:
                                    item.ImagePath3 = @"C:\AWC.DigitalCommerce\Images\GreenCheck1.png";
                                    break;
                            }

                            switch (item.ItemUnitOfMeasurement)
                            {
                                case 0:
                                    item.ImagePath4 = @"C:\AWC.DigitalCommerce\Images\u-64.png";
                                    break;
                                case 1:
                                    item.ImagePath4 = @"C:\AWC.DigitalCommerce\Images\m-64.png";
                                    break;
                                case 2:
                                    item.ImagePath4 = @"C:\AWC.DigitalCommerce\Images\g-64.png";
                                    break;
                            }

                            lstItems.Add(item);
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> ListBindingInventory_tbl_Items(int itemType)
        {
            try
            {
                List<clsItem> lstItems = new List<clsItem>();

                string sqlQuery = string.Empty;

                switch (itemType)
                {
                    case 0:
                        sqlQuery = "SELECT * FROM tbl_Items WHERE ItemType <> 9 ORDER BY ItemDescription ASC";
                        break;
                    case 1:
                    case 2:
                    case 3:
                        sqlQuery = $"SELECT * FROM tbl_Items WHERE IsActive = 1 AND ItemSubType <> 3 AND ItemType = {itemType}";
                        break;
                    default:
                        return null;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();
                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.ItemType = Convert.ToInt32(sdr["ItemType"]);
                            item.ItemSubType = Convert.ToInt32(sdr["ItemSubType"]);
                            item.ItemDescription = sdr["ItemDescription"].ToString();
                            item.IsActive = Convert.ToBoolean(sdr["IsActive"]);
                            item.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            item.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                            item.ItemAvailable = Convert.ToInt32(sdr["ItemAvailable"]);
                            item.ItemSold = Convert.ToInt32(sdr["ItemSold"]);
                            item.ItemDefective = Convert.ToInt32(sdr["ItemDefective"]);
                            item.DebitNotes = Convert.ToInt32(sdr["DebitNotes"]);
                            item.CreditNotes = Convert.ToInt32(sdr["CreditNotes"]);
                            item.ItemParent = Convert.ToInt32(sdr["ItemParent"]);
                            item.ItemMinimum = Convert.ToInt32(sdr["ItemMinimum"]);
                            item.ItemStock = Convert.ToInt32(sdr["ItemStock"]);
                            lstItems.Add(item);
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<string> ListBinding_tbl_ItemSubType(int itemType, int itemSubType)
        {
            try
            {
                List<string> lstItems = new List<string>();

                string sqlQuery = string.Empty;

                if (itemType == 0)
                {
                    sqlQuery = $"SELECT ItemDescription FROM tbl_Items WHERE IsActive = 1 AND ItemSubType = {itemSubType}";
                }
                else if (itemSubType == 100)
                {
                    sqlQuery = $"SELECT ItemDescription FROM tbl_Items WHERE IsActive = 1 AND ItemType = {itemType} AND ItemSubType = 0 AND ItemParent = 0";
                }
                else
                {
                    sqlQuery = $"SELECT ItemDescription FROM tbl_Items WHERE IsActive = 1 AND ItemType = {itemType} AND ItemSubType = {itemSubType}";
                }
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            lstItems.Add(sdr["ItemDescription"].ToString());
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> ListBinding_tbl_ParentItems()
        {
            try
            {
                List<clsItem> lstItems = new List<clsItem>();

                string sqlQuery = $"SELECT * FROM tbl_Items WHERE ItemType = 2 AND ItemSubType = 1";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();
                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.ItemDescription = sdr["ItemDescription"].ToString();
                            item.ItemParent = Convert.ToInt32(sdr["ItemParent"]);
                            item.ItemParentDescription = DB.GetItemDescriptionByItemID(item.ItemParent);
                            item.ItemParentUnit = Convert.ToInt32(sdr["ItemParentUnit"]);
                            lstItems.Add(item);
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> ListBinding_tbl_MealsRelationships()
        {
            try
            {
                List<clsItem> lstItems = new List<clsItem>();

                string sqlQuery = "SELECT * FROM tbl_MealsRelationships";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem itemFrom = DB.GetItem(Convert.ToInt32(sdr["ItemFrom"]));
                            clsItem itemTo = DB.GetItem(Convert.ToInt32(sdr["ItemTo"]));

                            clsItem item = new clsItem();
                            item.ID = itemFrom.ID;
                            item.ItemDescription = itemFrom.ItemDescription;
                            item.ItemParent = itemTo.ID;
                            item.ItemParentDescription = itemTo.ItemDescription;
                            item.ItemParentUnit = Convert.ToInt32(sdr["Qty"]);
                            lstItems.Add(item);
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> DataBinding_tbl_Tickets(string dt, int option)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsTicketsForDataGrid> Tickets = new List<clsTicketsForDataGrid>();

                switch(option)
                {
                    case 0:
                        // search all open tickets
                        sqlQry = "SELECT * FROM tbl_Tickets WHERE Status = 1 ORDER BY ID ASC";
                        break;
                    case 1:
                        // search for old tickets
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate <> '{dt}' AND Status = 1 ORDER BY ID ASC";
                        break;
                    case 2:
                        // search for todays
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate = '{dt}' AND Status = 0 ORDER BY ID ASC";
                        break;
                    case 3:
                        // search for todays sales no matter the status
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate = '{dt}' ORDER BY ID ASC";
                        break;
                    case 4:
                        // search for tickets open today
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate = '{dt}' AND Status = 1 ORDER BY ID ASC";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

                            ticket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            ticket.ID = Convert.ToInt32(sdr["ID"]);
                            ticket.Status = Convert.ToBoolean(sdr["Status"]);
                            ticket.Cash = Convert.ToInt32(sdr["Cash"]);
                            ticket.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                            ticket.Transfer = Convert.ToInt32(sdr["Transfer"]);
                            ticket.Voucher = Convert.ToInt32(sdr["Voucher"]);
                            ticket.PayMethod = Convert.ToInt32(sdr["PayMethod"]);
                            ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                            ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            ticket.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                            ticket.Splited = Convert.ToInt32(sdr["Splited"]);
                            ticket.Shift = Convert.ToInt32(sdr["Shift"]);

                            switch (ticket.PayMethod)
                            {
                                case 0:
                                    ticket.StatusAlpha = "ABIE";
                                    break;
                                case 1:
                                    ticket.StatusAlpha = "CANC";
                                    break;
                                case 2:
                                    ticket.StatusAlpha = "ANUL";
                                    break;
                                case 3:
                                    ticket.StatusAlpha = "HERE";
                                    break;
                            }

                            if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0 && ticket.Voucher == 0)
                            {
                                ticket.PayMethodAlpha = "EFECT";
                            }
                            else
                            if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0 && ticket.Voucher == 0)
                            {
                                ticket.PayMethodAlpha = "TCRED";
                            }
                            else
                            if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                            {
                                ticket.PayMethodAlpha = "SINPE";
                            }
                            else
                            if (ticket.Cash > 0 || ticket.CreditCard > 0 || ticket.Transfer > 0 && ticket.Voucher > 0)
                            {
                                ticket.PayMethodAlpha = "MIXTO";
                            }
                            else
                            {
                                if (ticket.PayMethod == 0)
                                {
                                    ticket.PayMethodAlpha = "PEND";
                                }
                                if (ticket.PayMethod > 1)
                                {
                                    ticket.PayMethodAlpha = ticket.StatusAlpha;
                                }
                                else
                                {
                                    if (ticket.StatusAlpha == "ABIE")
                                    {
                                        ticket.PayMethodAlpha = "PEND";
                                    }
                                    else
                                    {
                                        ticket.PayMethodAlpha = "MIXTO";
                                    }
                                }
                            }

                            ticket.CustomerAKA = sdr["CustomerAKA"].ToString();

                            if (ticket.CustomerAKA.Equals("ND"))
                                ticket.CustomerID = GetCustomerIDByID(Convert.ToInt32(sdr["CustomerID"]));
                            else
                                ticket.CustomerID = ticket.CustomerAKA;

                            Tickets.Add(ticket);
                        }
                    }
                }
                return Tickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> DataBinding_tbl_Tickets(string dt, int option, int custID)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsTicketsForDataGrid> Tickets = new List<clsTicketsForDataGrid>();

                switch (option)
                {
                    case 0:
                        // search all open tickets
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE CustomerID = {custID} AND Status = 1 ORDER BY TicketDate, ID ASC";
                        break;
                    case 1:
                        // search for old tickets
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate <> '{dt}' AND CustomerID = {custID} AND Status = 1 ORDER BY TicketDate, ID ASC";
                        break;
                    case 2:
                        // search for todays
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate = '{dt}' AND CustomerID = {custID} AND Status = 1 ORDER BY TicketDate, ID ASC";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

                            ticket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            ticket.ID = Convert.ToInt32(sdr["ID"]);
                            ticket.CustomerID = GetCustomerIDByID(Convert.ToInt32(sdr["CustomerID"]));
                            ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                            ticket.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                            ticket.CustomerAKA = sdr["CustomerAKA"].ToString();
                            Tickets.Add(ticket);
                        }
                    }
                }
                return Tickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> DataBinding_tbl_Tickets(int custID, int option)
        {
            try
            {
                List<clsTicketsForDataGrid> Tickets = new List<clsTicketsForDataGrid>();

                string sqlQry = string.Empty;

                switch(option)
                {
                    case 1:
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE CustomerID = {custID} AND Status = {option} ORDER BY TicketDate, ID";
                        break;
                    case 2:
                        sqlQry = "SELECT * FROM tbl_Tickets WHERE CustomerID = " + custID + " ORDER BY TicketDate, ID";
                        break;
                    case 3:
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE CustomerID = {custID} AND Status = 1 AND TicketDate <> '{Settings.Default.BusinessDate}' ORDER BY TicketDate, ID";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

                            ticket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            ticket.ID = Convert.ToInt32(sdr["ID"]);
                            ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                            ticket.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                            Tickets.Add(ticket);
                        }
                    }
                }
                return Tickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsSalesHistory> DataBinding_tbl_Tickets(string startDate, string endDate)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsSalesHistory> salesHist = new List<clsSalesHistory>();

                sqlQry = "SELECT TicketDate, SUM(TotalPrice) AS 'TotalSale' FROM tbl_Tickets WHERE TicketDate >= '" + startDate + "' AND TicketDate <= '" + endDate + "' " +
                         "GROUP BY TicketDate ORDER BY TicketDate ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsSalesHistory saleDay = new clsSalesHistory();

                            saleDay.salesDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            saleDay.salesTotal = Convert.ToInt32(sdr["TotalSale"]);
                            salesHist.Add(saleDay);
                        }
                    }
                }
                return salesHist;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> DataBinding_tbl_DailyClose(string workDay)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsTicketsForDataGrid> Tickets = new List<clsTicketsForDataGrid>();

                sqlQry = $"SELECT * FROM tbl_Tickets WHERE Shift = {Settings.Default.ShiftForQuery} AND TicketDate = '{workDay}' ORDER BY ID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();
                                
                            ticket.ID = Convert.ToInt32(sdr["ID"]);
                            ticket.CustomerID = GetCustomerIDByID(Convert.ToInt32(sdr["CustomerID"]));
                            ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                            ticket.Cash = Convert.ToInt32(sdr["Cash"]);
                            ticket.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                            ticket.Transfer = Convert.ToInt32(sdr["Transfer"]);
                            ticket.Voucher = Convert.ToInt32(sdr["Voucher"]);
                            ticket.PayMethod = Convert.ToInt32(sdr["PayMethod"]);

                            switch (ticket.PayMethod)
                            {
                                case 0:
                                    ticket.StatusAlpha = "ABIE";
                                    break;
                                case 1:
                                    ticket.StatusAlpha = "CANC";
                                    break;
                                case 2:
                                    ticket.StatusAlpha = "ANUL";
                                    break;
                                case 3:
                                    ticket.StatusAlpha = "HERE";
                                    break;
                            }

                            if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0 && ticket.Voucher == 0)
                            {
                                ticket.PayMethodAlpha = "EFECT";
                            }
                            else
                            if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0 && ticket.Voucher == 0)
                            {
                                ticket.PayMethodAlpha = "TCRED";
                            }
                            else
                            if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0 && ticket.Voucher == 0)
                            {
                                ticket.PayMethodAlpha = "SINPE";
                            }
                            else
                            if (ticket.Cash > 0 || ticket.CreditCard > 0 || ticket.Transfer > 0 && ticket.Voucher > 0)
                            {
                                ticket.PayMethodAlpha = "MIXTO";
                            }
                            else
                            {
                                if (ticket.PayMethod == 0)
                                {
                                    ticket.PayMethodAlpha = "PEND";
                                }
                                if (ticket.PayMethod > 1)
                                {
                                    ticket.PayMethodAlpha = ticket.StatusAlpha;
                                }
                                else
                                {
                                    if (ticket.StatusAlpha == "ABIE")
                                    {
                                        ticket.PayMethodAlpha = "PEND";
                                    }
                                    else
                                    {
                                        ticket.PayMethodAlpha = "MIXTO";
                                    }
                                }
                            }

                            Tickets.Add(ticket);
                        }
                    }
                }
                return Tickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsForDataGrid> DataBinding_tbl_DailyClose(string workDay1, string workDay2)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsTicketsForDataGrid> Tickets = new List<clsTicketsForDataGrid>();

                sqlQry = "SELECT * FROM tbl_Tickets WHERE TicketDate >= '" + workDay1 + "' AND TicketDate <= '" + workDay2 + "' ORDER BY ID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

                            ticket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            ticket.ID = Convert.ToInt32(sdr["ID"]);
                            ticket.CustomerID = GetCustomerIDByID(Convert.ToInt32(sdr["CustomerID"]));
                            ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                            ticket.Cash = Convert.ToInt32(sdr["Cash"]);
                            ticket.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                            ticket.Transfer = Convert.ToInt32(sdr["Transfer"]);
                            ticket.PayMethod = Convert.ToInt32(sdr["PayMethod"]);

                            switch (ticket.PayMethod)
                            {
                                case 0:
                                    ticket.PayMethodAlpha = "PEND";
                                    break;
                                case 1:
                                    ticket.PayMethodAlpha = "CANC";
                                    break;
                                case 2:
                                    ticket.PayMethodAlpha = "ANUL";
                                    break;
                                case 3:
                                    ticket.PayMethodAlpha = "HERE";
                                    break;
                            }
                            ticket.Status = Convert.ToBoolean(sdr["Status"]);
                            ticket.StatusAlpha = ticket.Status == true ? "PEND" : "CANC";
                            Tickets.Add(ticket);
                        }
                    }
                }
                return Tickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemType> DataBinding_tbl_TicketsDetail(string dt, int iType)
        {
            try
            {
                List<clsItemType> itemTypeList = new List<clsItemType>();

                string sqlQuery = "SELECT SUM(Qty) AS 'Qty', tbl_Items.ItemDescription AS 'ItemDesc' FROM tbl_TicketsDetail " +
                                  "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                                  "INNER JOIN tbl_Tickets ON tbl_TicketsDetail.GUID = tbl_Tickets.GUID " +
                                  "WHERE tbl_Items.ItemType = " + iType + " AND tbl_Tickets.TicketDate = '" + dt + "' " +
                                  "GROUP BY tbl_Items.ItemDescription";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemType itemType = new clsItemType();
                            itemType.Qty = Convert.ToInt32(sdr["Qty"]);
                            itemType.ItemDesc = sdr["ItemDesc"].ToString();
                            itemTypeList.Add(itemType);
                        }
                    }
                }
                return itemTypeList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemType> DataBinding_tbl_TicketsDetail(string dt, int iType, int max)
        {
            try
            {
                List<clsItemType> itemTypeList = new List<clsItemType>();

                string sqlQuery = "SELECT TOP " + max + " SUM(Qty) AS 'Qty', tbl_Items.ItemDescription AS 'ItemDesc' FROM tbl_TicketsDetail " +
                                  "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                                  "INNER JOIN tbl_Tickets ON tbl_TicketsDetail.GUID = tbl_Tickets.GUID " +
                                  "WHERE tbl_Items.ItemType = " + iType + " AND tbl_Tickets.TicketDate = '" + dt + "' " +
                                  "GROUP BY tbl_Items.ItemDescription ORDER BY Qty DESC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemType itemType = new clsItemType();
                            itemType.Qty = Convert.ToInt32(sdr["Qty"]);
                            itemType.ItemDesc = sdr["ItemDesc"].ToString();
                            itemTypeList.Add(itemType);
                        }
                    }
                }
                return itemTypeList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemType> DataBinding_tbl_TicketsDetail(string sd, string fd, int qry)
        {
            try
            {
                string sqlQuery = string.Empty;
                List <clsItemType> itemTypeList = new List<clsItemType>();

                switch (qry)
                {
                    case 0:
                        sqlQuery = "SELECT TOP 10 sum(Qty) AS 'Qty', I.ItemDescription AS 'ItemDesc', sum(TD.TotalPrice) as 'TotalPrice' " +
                                   "FROM tbl_TicketsDetail TD " + "" +
                                   "INNER JOIN tbl_Items I ON TD.ItemID = I.ID " +
                                   "INNER JOIN tbl_Tickets T ON TD.GUID = T.GUID " +
                                  $"WHERE T.TicketDate BETWEEN '{sd}' AND '{fd}' " +
                                   "GROUP BY I.ID, I.ItemDescription " +
                                   "ORDER BY Qty DESC";
                        break;
                    case 1:
                        sqlQuery = "SELECT TOP 10 sum(Qty) AS 'Qty', I.ItemDescription AS 'ItemDesc', sum(TD.TotalPrice) as 'TotalPrice' " +
                                   "FROM tbl_TicketsDetail TD " + "" +
                                   "INNER JOIN tbl_Items I ON TD.ItemID = I.ID " +
                                   "INNER JOIN tbl_Tickets T ON TD.GUID = T.GUID " +
                                  $"WHERE T.TicketDate BETWEEN '{sd}' AND '{fd}' " +
                                   "GROUP BY I.ID, I.ItemDescription " +
                                   "ORDER BY TotalPrice DESC";
                        break;
                }
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemType itemType = new clsItemType();
                            itemType.Qty = Convert.ToInt32(sdr["Qty"]);
                            itemType.ItemDesc = sdr["ItemDesc"].ToString();
                            itemType.TotalPrice = Convert.ToInt32(sdr["TotalPrice"].ToString());
                            itemTypeList.Add(itemType);
                        }
                    }
                }
                return itemTypeList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsProvider> ListBinding_tbl_Providers()
        {
            try
            {
                List<clsProvider> providersList = new List<clsProvider>();

                string sqlQuery = "SELECT ID, ProviderName FROM tbl_Providers";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsProvider provider = new clsProvider();
                            provider.ID = Convert.ToInt32(sdr["ID"]);
                            provider.ProviderName = sdr["ProviderName"].ToString();

                            providersList.Add(provider);
                        }
                    }
                }
                return providersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicket> ListBinding_tbl_TicketsAborted(string startDate, string endDate)
        {
            try
            {
                List<clsTicket> ticketsList = new List<clsTicket>();

                string sqlQuery = "SELECT tbl_TicketsAborted.TicketDate AS 'TicketDate', tbl_TicketsAborted.ID AS 'TicketNumber', tbl_TicketsAborted.GUID AS 'GUID', tbl_CustomerID.CustomerID AS 'CustomerAKA', TotalPrice AS 'TotalPrice', tbl_TicketsAborted.WhoOpened AS 'WhoOpened', tbl_TicketsAborted.WhoClosed AS 'WhoClosed', tbl_TicketsAborted.CloseAt AS 'CloseAt', tbl_TicketsAborted.AbortReason AS 'AbortReason' FROM tbl_TicketsAborted " +
                                  "INNER JOIN tbl_CustomerID ON tbl_CustomerID.ID = tbl_TicketsAborted.CustomerID " +
                                  "WHERE tbl_TicketsAborted.TicketDate >= '" + startDate + "' AND tbl_TicketsAborted.TicketDate <= '" + endDate + "' " +
                                  "ORDER BY tbl_TicketsAborted.TicketDate, tbl_TicketsAborted.ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicket abortedTicket = new clsTicket();

                            abortedTicket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            abortedTicket.ID = Convert.ToInt32(sdr["TicketNumber"]);
                            abortedTicket.GUID = sdr["GUID"].ToString();
                            abortedTicket.CustomerAKA = sdr["CustomerAKA"].ToString();
                            abortedTicket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            abortedTicket.WhoOpened = Convert.ToInt32(sdr["WhoOpened"]);
                            abortedTicket.WhoClosed = Convert.ToInt32(sdr["WhoClosed"]);
                            abortedTicket.CloseAt = Convert.ToDateTime(sdr["CloseAt"]);
                            abortedTicket.AbortReason = sdr["AbortReason"].ToString();
                            ticketsList.Add(abortedTicket);
                        }
                    }
                }
                return ticketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsInherited> ListBinding_tbl_TicketsInherited(string startDate, string endDate)
        {
            try
            {
                List<clsTicketsInherited> ticketsList = new List<clsTicketsInherited>();

                string sqlQuery = $"SELECT * FROM tbl_TicketsInherited WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' ORDER BY TicketDate, TicketID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsInherited inheritedTicket = new clsTicketsInherited();

                            inheritedTicket.ID = Convert.ToInt32(sdr["ID"].ToString());
                            inheritedTicket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            inheritedTicket.TicketID = Convert.ToInt32(sdr["TicketID"].ToString());
                            inheritedTicket.TicketGUID = sdr["TicketGUID"].ToString();
                            inheritedTicket.FromCustomer = sdr["FromCustomer"].ToString();
                            inheritedTicket.ToCustomer = sdr["ToCustomer"].ToString();
                            inheritedTicket.WhoMakeIt = sdr["WhoMakeIt"].ToString();
                            inheritedTicket.CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]);

                            ticketsList.Add(inheritedTicket);
                        }
                    }
                }
                return ticketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketsReassigned> ListBinding_tbl_TicketsReassigned(string startDate, string endDate)
        {
            try
            {
                List<clsTicketsReassigned> ticketsList = new List<clsTicketsReassigned>();

                string sqlQuery = $"SELECT * FROM tbl_TicketsReassigned WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' ORDER BY TicketDate, TicketID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketsReassigned reassignedTicket = new clsTicketsReassigned();

                            reassignedTicket.ID = Convert.ToInt32(sdr["ID"]);
                            reassignedTicket.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            reassignedTicket.TicketID = Convert.ToInt32(sdr["TicketID"]);
                            reassignedTicket.FromCustomer = sdr["FromCustomer"].ToString();
                            reassignedTicket.ToCustomer = sdr["ToCustomer"].ToString();
                            reassignedTicket.WhoMakeIt = sdr["WhoMakeIt"].ToString();
                            reassignedTicket.CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]);

                            ticketsList.Add(reassignedTicket);
                        }
                    }
                }
                return ticketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDeleted> ListBinding_tbl_ItemsDeleted(string startDate, string endDate)
        {
            try
            {
                List<clsItemDeleted> itemsList = new List<clsItemDeleted>();

                string sqlQuery = $"SELECT * FROM tbl_ItemsDeleted WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' ORDER BY DeletedAt ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDeleted itemDeleted = new clsItemDeleted();

                            itemDeleted.ID = Convert.ToInt32(sdr["ID"]);
                            itemDeleted.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            itemDeleted.ItemDescription = GetItemDescriptionByItemID(Convert.ToInt32(sdr["ItemID"]));
                            itemDeleted.Qty = Convert.ToInt32(sdr["Qty"]);
                            itemDeleted.WhoDeleted = Convert.ToInt32(sdr["WhoDeleted"]);
                            clsUser checkPIN = CheckUserPIN(sdr["WhoDeleted"].ToString());
                            itemDeleted.WhoDeletedName = checkPIN.userName;
                            checkPIN = CheckUserPIN(sdr["WhoAuth"].ToString());
                            itemDeleted.WhoAuthName = checkPIN.userName;
                            itemDeleted.DeletedAt = Convert.ToDateTime(sdr["DeletedAt"]);
                            itemDeleted.DeletedAtString = itemDeleted.DeletedAt.ToString("dd/MM/yyyy HH:mm:ss");

                            itemsList.Add(itemDeleted);
                        }
                    }
                }
                return itemsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDeletedFromSystem> ListBinding_tbl_ItemsDeletedFromSystem(string startDate, string endDate)
        {
            try
            {
                List<clsItemDeletedFromSystem> itemsList = new List<clsItemDeletedFromSystem>();

                string sqlQuery = $"SELECT * FROM tbl_ItemsDeletedFromSystem WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' ORDER BY DeletedAt ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDeletedFromSystem idfs = new clsItemDeletedFromSystem();

                            idfs.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            idfs.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            idfs.ItemDescription = sdr["ItemDescription"].ToString();
                            idfs.WhoDeletedName = sdr["WhoDeletedName"].ToString();
                            idfs.DeletedAt = Convert.ToDateTime(sdr["DeletedAt"]);
                            idfs.DeletedAtString = idfs.DeletedAt.ToString("dd-MM-yyyy HH:mm:ss");

                            itemsList.Add(idfs);
                        }
                    }
                }
                return itemsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region UTILITIES
        public static string ConverTicketDate(string dt)
        {
            try
            {
                return dt.Substring(6, 2) + "." + dt.Substring(4, 2) + "." + dt.Substring(0, 4);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "ERROR: " + ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static string AWCDigitalCommerceDBBackup()
        {
            try
            {
                string dbName = AWCDigitalCommerceDatabaseBName();

                string bakName = Path.Combine(Settings.Default.DatabaseBackupLocation, dbName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.bak"));

                if (!Directory.Exists(Settings.Default.DatabaseBackupLocation))
                    Directory.CreateDirectory(Settings.Default.DatabaseBackupLocation);

                string sqlQry = $"BACKUP DATABASE {dbName} TO DISK = '{bakName}' WITH INIT , NOUNLOAD , NAME = N'{dbName} Backup', NOSKIP , STATS = 10, NOFORMAT";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return bakName;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return string.Empty;
            }
        }
        public static string AWCDigitalCommerceDatabaseBName()
        {
            try
            {
                string dbName = string.Empty;
                string sqlQry = "exec sp_spaceused";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        // the SP returns two records, so read just the first column of first record
                        sdr.Read();
                        dbName = sdr.GetString(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return dbName;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        #endregion

        #region GENERAL
        public static clsUser CheckUserPIN(string userPIN)
        {
            try
            {
                clsUser userProfile = new clsUser();

                userProfile.userActive = false;
                userProfile.userName = string.Empty;

                string sqlQry = "SELECT * FROM tbl_Users WHERE userPIN = '" + userPIN + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        userProfile.userID = Convert.ToInt32(sdr["userID"]);
                        userProfile.userDTCreation = Convert.ToDateTime(sdr["userDTCreation"]);
                        userProfile.userPIN = sdr["userPIN"].ToString();
                        userProfile.userPW = sdr["userPW"].ToString();
                        userProfile.userName = sdr["userName"].ToString().ToUpper();
                        userProfile.userAccessLevel= sdr["userAccessLevel"].ToString();
                        userProfile.userActive = Convert.ToBoolean(sdr["userActive"]);
                        userProfile.userSecurityProfile = sdr["userSecurityProfile"].ToString();
                        userProfile.userPowerAdmin = Convert.ToBoolean(sdr["userPowerAdmin"]);
                    }
                }
                
                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return userProfile;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static void UpdateUserSecurityProfile(clsUser userProf)
        {
            try
            {
                int active = userProf.userActive ? 1 : 0;
                int powerAdmin = userProf.userPowerAdmin ? 1 : 0;

                string sqlQry = "UPDATE tbl_Users SET userName = '" + userProf.userName +
                                "', userAccessLevel = '" + userProf.userAccessLevel +
                                "', userActive = " + active + ", " +
                                "userPowerAdmin = " + powerAdmin + ", " +
                                "userSecurityProfile = '" + userProf.userSecurityProfile +
                                "' WHERE userPIN = '" + userProf.userPIN + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static bool CheckOriginDestinyRelation(string _orig, string _dest)
        {
            try
            {
                bool result = false;

                int origID = GetIDByItemDescription(_orig);
                int destID = GetIDByItemDescription(_dest);

                string sqlQry = "SELECT ID FROM tbl_Items WHERE ItemSubType = 1 AND ID = " + origID + " AND ItemParent = " + destID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        result = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return true;
            }
        }
        public static bool CheckMealOriginDestinyRelation(string _orig, string _dest)
        {
            try
            {
                bool result = false;

                int origID = GetIDByItemDescription(_orig);
                int destID = GetIDByItemDescription(_dest);

                string sqlQry = $"SELECT ID FROM tbl_MealsRelationships WHERE ItemFrom = {origID} AND ItemTo = {destID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        result = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return true;
            }
        }
        public static int TodayTicketsOpen(string dt)
        {
            try
            {
                int openTickets = 0;

                string sqlQry = "SELECT COUNT(*) FROM tbl_Tickets WHERE TicketDate = '" + dt + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        openTickets = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return openTickets;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static void NormalizeCustomerID()
        {
            try
            {
                string sqlQry = "UPDATE tbl_CustomerID SET Active = 0";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void ReverseNormalizeCustomerID(string workDay)
        {
            try
            {
                string sqlQry = "UPDATE tbl_CustomerID SET Active = 1 " +
                                "WHERE ID IN (SELECT CustomerID FROM tbl_DailyClosing WHERE WorkDay = '" + workDay + "') " +
                                "DELETE tbl_DailyClosing WHERE WorkDay = '" + workDay + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static void MoveOpenTicketsToDailyClosing(string workDay)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_DailyClosing (WorkDay, CustomerID, CustomerAKA, TicketNumber) " +
                                "SELECT TicketDate, CustomerID, customerAKA, ID FROM tbl_Tickets " +
                                "WHERE Status = 1 AND tbl_Tickets.ID NOT IN (SELECT TicketNumber FROM tbl_DailyClosing)";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static bool CustomerIDExist(string custID)
        {
            try
            {
                bool result = false;

                string sqlQry = "SELECT ID FROM tbl_CustomerID WHERE CustomerID = '" + custID.ToUpper() + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                        result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool TicketNotExist(string dt, int custID)
        {
            try
            {
                bool result = false;

                string sqlQry = "SELECT GUID FROM tbl_Tickets WHERE TicketDate = '" + dt + "' AND CustomerID = " + custID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                        result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool CheckInternalAccount(int ID)
        {
            try
            {
                bool status = false;

                string sqlQry = $"SELECT * FROM tbl_CustomerID WHERE TYPE = 3 AND ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        status = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return status;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool CheckDailyClosingSummary(string workday, int shift)
        {
            try
            {
                bool status = false;

                string sqlQry = $"SELECT * FROM tbl_DailyClosingSummary WHERE BusinessDate = {workday} AND Shift = {shift}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        status = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return status;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static void TurnOffPowerUserTickets(int ID)
        {
            try
            {
                string sqlQry = $"DELETE tbl_DailyClosing WHERE CustomerID = {ID}; UPDATE tbl_Tickets SET status = 0 WHERE CustomerID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }

        }
        public static bool TruncateTable(string tableName)
        {
            try
            {
                bool result = false;

                string sqlQry = $"TRUNCATE TABLE {tableName}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false; ;
            }

        }
        public static bool RebuildAllIndexes()
        {
            try
            {
                bool result = false;
                string sqlQry = File.ReadAllText("C:\\AWC.DigitalCommerce\\MSSQL\\RebuildAllIndexes.sql");

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false; ;
            }
        }
        #endregion

        #region GETS
        public static clsCustomerVIP GetCustomerProfile(string custID)
        {
            try
            {
                clsCustomerVIP custProf = new clsCustomerVIP();

                string sqlQry = "SELECT * FROM tbl_CustomerID WHERE CustomerID = '" + custID.ToUpper() + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        custProf.ID = Convert.ToInt32(sdr["ID"]);
                        custProf.Type = Convert.ToInt32(sdr["Type"]);
                        custProf.CustomerID = custID.ToUpper();
                        custProf.Active = Convert.ToBoolean(sdr["Active"]);
                        custProf.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                        custProf.LastPayment = sdr["LastPayment"].ToString();
                        custProf.CustomerFOC = Convert.ToBoolean(sdr["FreeOfCharge"]);
                        custProf.CreditLimit = Convert.ToInt32(sdr["CreditLimit"]);
                        custProf.BirthDay = sdr["BirthDay"].ToString();
                        custProf.MailAddress = sdr["MailAddress"].ToString();
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return custProf;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static int GetUnitPriceByItemDescription(string itemDesc)
        {
            try
            {
                int unitPrice = 0;

                string sqlQry = "SELECT UnitPrice FROM tbl_Items WHERE ItemDescription = '" + itemDesc + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        unitPrice = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return unitPrice;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static int GetUnitCostByItemDescription(string itemDesc)
        {
            try
            {
                int unitCost = 0;

                string sqlQry = "SELECT UnitCost FROM tbl_Items WHERE ItemDescription = '" + itemDesc + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        unitCost = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return unitCost;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static int GetIDByCustomerID(string customerID)
        {
            try
            {
                int ID = 0;

                string sqlQry = "SELECT ID FROM tbl_CustomerID WHERE CustomerID = '" + customerID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ID = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static string GetCustomerIDByID(int ID)
        {
            try
            {
                if (ID == 0)
                    return string.Empty;

                string custID = string.Empty;
                string sqlQry = string.Empty;

                if (Settings.Default.UseNickNames && filterON)
                    sqlQry = "SELECT CustomerID FROM tbl_OpenTickets WHERE ID = " + ID;
                else
                    sqlQry = "SELECT CustomerID FROM tbl_CustomerID WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        custID = sdr.GetString(0);
                    }
                    else if (!sdr.HasRows && Settings.Default.UseNickNames)
                    {
                        filterON = false;
                        custID = GetCustomerIDByID(ID);
                        filterON = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return custID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static int GetCustomerID(string custID)
        {
            try
            {
                int result = 0;

                string sqlQry = "SELECT ID FROM tbl_CustomerID WHERE CustomerID = '" + custID.ToUpper() + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        result = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static bool GetCustomerIDFromOpenTickets(string custID)
        {
            try
            {
                bool exist = false;

                string sqlQry = "SELECT CustomerID FROM tbl_OpenTickets WHERE CustomerID = '" + custID.ToUpper() + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                        exist = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return exist;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static int GetIDByItemDescription(string itemDesc)
        {
            try
            {
                int ID = 0;

                string sqlQry = "SELECT ID FROM tbl_Items WHERE ItemDescription = '" + itemDesc.ToUpper() + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ID = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static bool IsMealItemType(string itemDesc)
        {
            try
            {
                bool isMeal = false;

                string sqlQry = "SELECT ID FROM tbl_Items WHERE ItemDescription = '" + itemDesc + "' AND ItemType = 3";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        isMeal = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return isMeal;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static int GetItemSubtype(string itemDesc)
        {
            try
            {
                int itemSubtype = 0;

                string sqlQry = $"SELECT ItemSubType FROM tbl_Items WHERE ItemDescription = '{itemDesc}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        itemSubtype = Convert.ToInt32(sdr["ItemSubType"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return itemSubtype;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static string GetItemDescriptionByItemID(int itemID)
        {
            try
            {
                string itemDesc = string.Empty;

                string sqlQry = "SELECT ItemDescription FROM tbl_Items WHERE ID = " + itemID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        itemDesc = sdr.GetString(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return itemDesc;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return string.Empty;
            }
        }
        public static string GetTicketGUID(string dt, int custID, int status)
        {
            try
            {
                string GUID = string.Empty;

                string sqlQry = "SELECT GUID FROM tbl_Tickets WHERE TicketDate = '" + dt + "' AND Status = " + status + " AND CustomerID = " + custID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        GUID = sdr.GetString(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return GUID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return string.Empty;
            }
        }
        public static string GetTicketGUID(int ticketID)
        {
            try
            {
                string GUID = string.Empty;

                string sqlQry = "SELECT GUID FROM tbl_Tickets WHERE ID = " + ticketID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        GUID = sdr.GetString(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return GUID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return string.Empty;
            }
        }
        public static clsTicket GetTicket(int TicketNum)
        {
            try
            {
                clsTicket ticket = new clsTicket();

                string sqlQry = "SELECT * FROM tbl_Tickets WHERE ID = " + TicketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticket.ID = TicketNum;
                        ticket.TicketDate = sdr["TicketDate"].ToString();
                        ticket.GUID = sdr["GUID"].ToString();
                        ticket.CustID = Convert.ToInt32(sdr["CustomerID"]);
                        ticket.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                        ticket.ServiceFee = Convert.ToInt32(sdr["ServiceFee"]);
                        ticket.IVAFee = Convert.ToInt32(sdr["IVAFee"]);
                        ticket.Payments = Convert.ToInt32(sdr["Payments"]);
                        ticket.Cash = Convert.ToInt32(sdr["Cash"]);
                        ticket.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                        ticket.Transfer = Convert.ToInt32(sdr["Transfer"]);
                        ticket.CreateAt = Convert.ToDateTime(sdr["CreateAt"]);

                        if (sdr["CloseAt"] != System.DBNull.Value)
                        {
                            ticket.CloseAt = Convert.ToDateTime(sdr["CloseAt"]);
                        }

                        ticket.PayMethod = Convert.ToInt32(sdr["PayMethod"]);
                        ticket.Status = Convert.ToBoolean(sdr["Status"]);
                        ticket.WhoOpened = Convert.ToInt32(sdr["WhoOpened"]);
                        ticket.WhoClosed = Convert.ToInt32(sdr["WhoClosed"]);
                        ticket.Splited = Convert.ToBoolean(sdr["Splited"]);
                        ticket.CreateAt = Convert.ToDateTime(sdr["CreateAt"]);
                        ticket.CustomerAKA = sdr["customerAKA"].ToString();
                        ticket.ApplyServiceFee = Convert.ToBoolean(sdr["ApplyServiceFee"]);
                        ticket.AbortReason = sdr["AbortReason"].ToString();
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticket;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsTicketDetail GetSingleTickeDetail(int TicketDetailID)
        {
            try
            {
                clsTicketDetail ticketDetail = new clsTicketDetail();

                string sqlQry = $"SELECT * FROM tbl_TicketsDetail WHERE ID = {TicketDetailID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticketDetail.ID = TicketDetailID;
                        ticketDetail.GUID = sdr["GUID"].ToString();
                        ticketDetail.ItemID = Convert.ToInt32(sdr["ItemID"]);
                        ticketDetail.ItemDesc = GetItemDescriptionByItemID(Convert.ToInt32(ticketDetail.ItemID));
                        ticketDetail.Qty = Convert.ToInt32(sdr["Qty"]);
                        ticketDetail.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                        ticketDetail.TotalCost = Convert.ToInt32(sdr["TotalCost"]);
                        ticketDetail.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                        ticketDetail.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticketDetail;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsTicket GetTicketsSummary(string workDay)
        {
            try
            {
                clsTicket ticket = new clsTicket();

                string sqlQry = "SELECT SUM(Cash) AS 'Cash', SUM(CreditCard) AS 'CreditCard', SUM(Transfer) AS 'Transfer', SUM(Voucher) AS 'Voucher', " +
                                "SUM(ServiceFee) AS 'ServiceFee', SUM(TotalPrice) AS 'TotalPrice', " +
                                $"(SELECT SUM(TotalPrice) FROM tbl_Tickets WHERE Status = 1 AND TicketDate  = '{workDay}') AS 'OutstandingAmount' " +
                                $"FROM tbl_Tickets WHERE Status = 0 AND Shift = {Settings.Default.ShiftForQuery} AND TicketDate = '{workDay}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticket.Cash = DBNull.Value.Equals(sdr["Cash"]) ? 0 : Convert.ToInt32(sdr["Cash"]);
                        ticket.CreditCard = DBNull.Value.Equals(sdr["CreditCard"]) ? 0 : Convert.ToInt32(sdr["CreditCard"]);
                        ticket.Transfer = DBNull.Value.Equals(sdr["Transfer"]) ? 0 : Convert.ToInt32(sdr["Transfer"]);
                        ticket.Voucher = DBNull.Value.Equals(sdr["Voucher"]) ? 0 : Convert.ToInt32(sdr["Voucher"]);
                        ticket.ServiceFee = DBNull.Value.Equals(sdr["ServiceFee"]) ? 0 : Convert.ToInt32(sdr["ServiceFee"]);
                        ticket.Payments = DBNull.Value.Equals(sdr["OutstandingAmount"]) ? 0 : Convert.ToInt32(sdr["OutstandingAmount"]);
                        ticket.TotalPrice = DBNull.Value.Equals(sdr["TotalPrice"]) ? 0 : Convert.ToInt32(sdr["TotalPrice"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticket;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsTicket GetTicketsSummary(string workDay1, string workDay2)
        {
            try
            {
                clsTicket ticket = new clsTicket();

                string sqlQry = "SELECT SUM(Cash) AS 'Cash', SUM(CreditCard) AS 'CreditCard', SUM(Transfer) AS 'Transfer', " +
                                "SUM(ServiceFee) AS 'ServiceFee', SUM(TotalPrice) AS 'TotalPrice', " +
                                "(SELECT SUM(TotalPrice) FROM tbl_Tickets WHERE Status = 1 AND (TicketDate  >= '" + workDay1 + "' AND TicketDate  <= '" + workDay2 + "')) AS 'OutstandingAmount' " +
                                "FROM tbl_Tickets WHERE Status = 0 AND (TicketDate  >= '" + workDay1 + "' AND TicketDate  <= '" + workDay2 + "')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticket.Cash = DBNull.Value.Equals(sdr["Cash"]) ? 0 : Convert.ToInt32(sdr["Cash"]);
                        ticket.CreditCard = DBNull.Value.Equals(sdr["CreditCard"]) ? 0 : Convert.ToInt32(sdr["CreditCard"]);
                        ticket.Transfer = DBNull.Value.Equals(sdr["Transfer"]) ? 0 : Convert.ToInt32(sdr["Transfer"]);
                        ticket.ServiceFee = DBNull.Value.Equals(sdr["ServiceFee"]) ? 0 : Convert.ToInt32(sdr["ServiceFee"]);
                        ticket.Payments = DBNull.Value.Equals(sdr["OutstandingAmount"]) ? 0 : Convert.ToInt32(sdr["OutstandingAmount"]);
                        ticket.TotalPrice = DBNull.Value.Equals(sdr["TotalPrice"]) ? 0 : Convert.ToInt32(sdr["TotalPrice"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticket;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static int GetTicketNumber(string dt, int custID)
        {
            try
            {
                int ticketNum = 0;

                string sqlQry = "SELECT ID FROM tbl_Tickets WHERE TicketDate = '" + dt + "' AND Status = 1 AND CustomerID = " + custID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticketNum = sdr.GetInt32(0);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticketNum;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static bool GetTicketStatus(int ticketNum)
        {
            try
            {
                bool status = false;

                string sqlQry = "SELECT Status FROM tbl_Tickets WHERE ID = " + ticketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        status = Convert.ToBoolean(sdr["Status"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return status;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool GetTicketSplitedStatus(int ticketNum)
        {
            try
            {
                bool status = false;

                string sqlQry = "SELECT Splited FROM tbl_Tickets WHERE ID = " + ticketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        status = Convert.ToBoolean(sdr["Splited"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return status;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static clsTicket GetTickeyByCustomerID(string customerAKA)
        {
            try
            {
                clsTicket ticket = new clsTicket();

                string sqlQry = $"SELECT * FROM tbl_Tickets WHERE customerAKA = '{customerAKA}' AND Status = 1";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticket = DB.GetTicket(Convert.ToInt32(sdr["ID"]));
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticket;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static clsItem GetItem(int itemID)
        {
            try
            {
                clsItem item = new clsItem();

                string sqlQry = "SELECT * FROM tbl_Items WHERE ID = " + itemID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        item.ID = itemID;
                        item.ItemType = Convert.ToInt32(sdr["ItemType"]);
                        item.ItemSubType = Convert.ToInt32(sdr["ItemSubType"]);
                        item.ItemDescription = sdr["ItemDescription"].ToString();
                        item.IsActive = Convert.ToBoolean(sdr["IsActive"]);
                        item.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                        item.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                        item.ItemAvailable = Convert.ToInt32(sdr["ItemAvailable"]);
                        item.ItemSold = Convert.ToInt32(sdr["ItemSold"]);
                        item.ItemDefective = Convert.ToInt32(sdr["ItemDefective"]);
                        item.DebitNotes = Convert.ToInt32(sdr["DebitNotes"]);
                        item.CreditNotes = Convert.ToInt32(sdr["CreditNotes"]);
                        item.ItemParent = Convert.ToInt32(sdr["ItemParent"]);
                        item.ItemParentUnit = Convert.ToInt32(sdr["ItemParentUnit"]);
                        item.ItemMinimum = Convert.ToInt32(sdr["ItemMinimum"]);
                        item.ItemUnitOfMeasurement = Convert.ToInt32(sdr["ItemUnitOfMeasurement"]);
                        item.ItemUnitSize = Convert.ToInt32(sdr["ItemUnitSize"]);
                        item.ItemStock = Convert.ToInt32(sdr["ItemStock"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return item;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetItemsByGUID(string GUID, bool bSummary)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                if (bSummary)
                    //sqlQry = "SELECT 9999 AS 'ID', ItemType, ItemID, SUM(Qty) AS 'Qty', UnitCost, SUM(TotalCost) AS 'TotalCost', UnitPrice, SUM(TotalPrice) AS 'TotalPrice' FROM tbl_TicketsDetail " +
                    //         "WHERE GUID = '" + GUID + "' GROUP BY ItemType, ItemID, UnitCost, UnitPrice";
                    sqlQry = "SELECT 9999 AS 'ID', ItemType, ItemID, SUM(Qty) AS 'Qty', MAX(UnitCost) AS 'UnitCost', SUM(TotalCost) AS 'TotalCost', MAX(UnitPrice) AS 'UnitPrice', SUM(TotalPrice) AS 'TotalPrice' FROM tbl_TicketsDetail " +
                             "WHERE GUID = '" + GUID + "' GROUP BY ItemType, ItemID";
                else
                            sqlQry = "SELECT ID, ItemType, ItemID, Qty, UnitCost, TotalCost, UnitPrice, TotalPrice FROM tbl_TicketsDetail WHERE GUID = '" + GUID + "' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while(sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ID = Convert.ToInt32(sdr["ID"]);
                            detailItem.ItemID = Convert.ToInt32(sdr["ItemID"]);

                            if (detailItem.ItemID > 100000)
                            {                                
                                detailItem.ItemDesc = "CUENTA " + (detailItem.ItemID - 100000).ToString("000000"); ;
                                detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\a2p.png";
                            }
                            else
                            {
                                clsItem item = GetItem(detailItem.ItemID);

                                detailItem.ItemDesc = item.ItemDescription;

                                switch (item.ItemType)
                                {
                                    case 0:
                                        detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\NoAvailable.png";
                                        break;
                                    case 1:
                                        detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\beer.png";
                                        break;
                                    case 2:
                                        detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\liquors.ico";
                                        break;
                                    case 3:
                                        detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\kitchen.ico";
                                        break;
                                    case 9:
                                        detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\otherTrans.png";
                                        break;
                                }

                                if (item.ItemType == 1 && item.ItemSubType == 2)
                                {
                                    detailItem.ImagePath = @"C:\AWC.DigitalCommerce\Images\beer-bottle.png";
                                }
                            }

                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                            detailItem.TotalCost = Convert.ToInt32(sdr["TotalCost"]);
                            detailItem.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);

                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetItemsAbortedByGUID(string GUID, bool bSummary)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                if (bSummary)
                    sqlQry = "SELECT 9999 AS 'ID', ItemID, SUM(Qty) AS 'Qty', UnitCost, SUM(TotalCost) AS 'TotalCost', UnitPrice, SUM(TotalPrice) AS 'TotalPrice' FROM tbl_TicketsDetailAborted " +
                             "WHERE GUID = '" + GUID + "' GROUP BY ItemID, UnitCost, UnitPrice";
                else
                    sqlQry = "SELECT ID, ItemID, Qty, UnitCost, TotalCost, UnitPrice, TotalPrice FROM tbl_TicketsDetailAborted WHERE GUID = '" + GUID + "' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ID = Convert.ToInt32(sdr["ID"]);
                            detailItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            detailItem.ItemDesc = GetItemDescriptionByItemID(detailItem.ItemID);
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                            detailItem.TotalCost = Convert.ToInt32(sdr["TotalCost"]);
                            detailItem.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);

                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetItemsByGUIDWithoutSplited(string GUID)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = "SELECT ID, ItemID, Qty, UnitPrice, TotalPrice FROM tbl_TicketsDetail WHERE GUID = '" + GUID + "' AND Splited = 0 ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ID = Convert.ToInt32(sdr["ID"]);
                            detailItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            detailItem.ItemDesc = GetItemDescriptionByItemID(detailItem.ItemID);
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);

                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetItemsByDate(string startDate, string finishDate, int itemType)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = String.Empty;

                switch(itemType)
                {
                    case 1:
                    case 2:
                    case 3:
                        sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitPrice, SUM(tbl_TicketsDetail.TotalPrice) AS 'TotalPrice', tbl_Items.ItemAvailable AS 'ItemAvail', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                                 "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                                 "WHERE tbl_Items.ItemType <> 9 AND tbl_Items.ItemType = " + itemType + " AND (tbl_TicketsDetail.CreatedAt >= '" + startDate + "' AND tbl_TicketsDetail.CreatedAt <= '" + finishDate + "') " +
                                 "GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitPrice, tbl_Items.ItemAvailable " +
                                 "ORDER BY tbl_Items.ItemDescription";
                        break;
                    case 4:
                        sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitPrice, SUM(tbl_TicketsDetail.TotalPrice) AS 'TotalPrice', tbl_Items.ItemAvailable AS 'ItemAvail', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                                $"INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                                $"INNER JOIN tbl_Tickets ON tbl_Tickets.GUID = tbl_TicketsDetail.GUID " +
                                $"WHERE tbl_Tickets.Shift = {Settings.Default.ShiftForQuery} AND tbl_Items.ItemType <> 9 AND (tbl_TicketsDetail.CreatedAt >= '{startDate}' AND tbl_TicketsDetail.CreatedAt <= '{finishDate}') " +
                                "GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitPrice, tbl_Items.ItemAvailable ORDER BY tbl_Items.ItemDescription";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.ItemDesc = sdr["ItemDesc"].ToString();
                            detailItem.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            detailItem.ItemAvailable = Convert.ToInt32(sdr["ItemAvail"]);
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetMealsItemsByDate(string dt)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = string.Empty;

                if (Settings.Default.KitchenLeasingService)
                {
                    sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitCost AS 'UnitCost', SUM(tbl_TicketsDetail.TotalCost) AS 'TotalCost', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                             "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                             "WHERE (tbl_Items.ItemType = 3 AND tbl_TicketsDetail.CreatedAt = '" + dt + "') GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitCost " +
                             "ORDER BY tbl_Items.ItemDescription";
                }
                else
                {
                    sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitPrice AS 'UnitPrice', SUM(tbl_TicketsDetail.TotalPrice) AS 'TotalPrice', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                             "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                             "WHERE (tbl_Items.ItemType = 3 AND tbl_TicketsDetail.CreatedAt = '" + dt + "') GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitPrice " +
                             "ORDER BY tbl_Items.ItemDescription";
                }
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ItemDesc = sdr["ItemDesc"].ToString();
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);

                            if (Settings.Default.KitchenLeasingService)
                            {
                                detailItem.UnitCost = Convert.ToInt32(sdr["UnitCost"]);
                                detailItem.TotalCost = Convert.ToInt32(sdr["TotalCost"]);
                            }
                            else
                            {
                                detailItem.UnitCost = Convert.ToInt32(sdr["UnitPrice"]);
                                detailItem.TotalCost = Convert.ToInt32(sdr["TotalPrice"]);
                            }
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetBeveragesItemsByDate(string dt)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = string.Empty;

                sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitPrice AS 'UnitPrice', SUM(tbl_TicketsDetail.TotalPrice) AS 'TotalPrice', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                         "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                         "WHERE (tbl_Items.ItemType = 1 AND tbl_TicketsDetail.CreatedAt = '" + dt + "') GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitPrice " +
                         "ORDER BY tbl_Items.ItemDescription";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ItemDesc = sdr["ItemDesc"].ToString();
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitCost = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalCost = Convert.ToInt32(sdr["TotalPrice"]);
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetLiquorsItemsByDate(string dt)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = string.Empty;

                sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(Qty) AS 'Qty', tbl_TicketsDetail.UnitPrice AS 'UnitPrice', SUM(tbl_TicketsDetail.TotalPrice) AS 'TotalPrice', tbl_Items.ItemType AS 'ItemType' FROM tbl_TicketsDetail " +
                         "INNER JOIN tbl_Items ON tbl_TicketsDetail.ItemID = tbl_Items.ID " +
                         "WHERE (tbl_Items.ItemType = 2 AND tbl_TicketsDetail.CreatedAt = '" + dt + "') GROUP BY tbl_Items.ItemDescription, tbl_Items.ItemType, tbl_TicketsDetail.UnitPrice " +
                         "ORDER BY tbl_Items.ItemDescription";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ItemDesc = sdr["ItemDesc"].ToString();
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitCost = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalCost = Convert.ToInt32(sdr["TotalPrice"]);
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetInventoryByDate(string startDate, string endDate)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = string.Empty;

                sqlQry = "SELECT tbl_Items.ItemDescription AS 'ItemDesc', SUM(ItemQty) AS 'Qty' FROM tbl_InvoicesDetail " +
                         "INNER JOIN tbl_Items ON tbl_InvoicesDetail.ItemID = tbl_Items.ID " +
                         $"WHERE tbl_InvoicesDetail.InvoiceGUID IN (SELECT InvoiceGUID FROM tbl_Invoices WHERE InvoiceDate >= '{startDate}' AND InvoiceDate <= '{endDate}') " +
                         "GROUP BY tbl_Items.ItemDescription ORDER BY tbl_Items.ItemDescription";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ItemDesc = sdr["ItemDesc"].ToString();
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsCustFreqItem> GetCustomerFrequentItems(int customerID)
        {
            try
            {
                List<clsCustFreqItem> custFreqItems = new List<clsCustFreqItem>();

                string sqlQry = string.Empty;

                if (customerID == 0)
                {
                    sqlQry = "SELECT TOP " + Settings.Default.NumOfCustomerFrequentItems + " COUNT(Qty) as Qty, tbl_Items.ItemType, ItemID, tbl_Items.ItemDescription FROM tbl_TicketsDetail " +
                             "INNER JOIN tbl_Items ON tbl_Items.ID = tbl_TicketsDetail.ItemID " +
                             "WHERE tbl_Items.ItemType <> 9 GROUP BY tbl_Items.ItemType, ItemID, tbl_Items.ItemDescription ORDER BY Qty DESC";
                }
                else
                {
                    sqlQry = "SELECT TOP " + Settings.Default.NumOfCustomerFrequentItems + " COUNT(Qty) as Qty, tbl_Items.ItemType, ItemID, tbl_Items.ItemDescription FROM tbl_TicketsDetail " +
                             "INNER JOIN tbl_Items ON tbl_Items.ID = tbl_TicketsDetail.ItemID " +
                             "WHERE tbl_Items.ItemType <> 9 AND GUID IN(SELECT GUID FROM tbl_Tickets WHERE CustomerID = " + customerID + ")" +
                             "GROUP BY tbl_Items.ItemType, ItemID, tbl_Items.ItemDescription ORDER BY Qty DESC";
                }
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCustFreqItem custFreqItem = new clsCustFreqItem();

                            custFreqItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            custFreqItem.ItemType = Convert.ToInt32(sdr["ItemType"]);
                            custFreqItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            custFreqItem.ItemDescription = sdr["ItemDescription"].ToString();

                            custFreqItems.Add(custFreqItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return custFreqItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsDelincuency> GetDelincuencies(string dt)
        {
            try
            {
                int custID = 0;
                string custName = string.Empty;
                int sum_0_8_days = 0;
                int sum_9_15_days = 0;
                int sum_16_30_days = 0;
                int sum_31_45_days = 0;
                int sum_46_60_days = 0;
                int sum_61_days = 0;
                bool firstRec = true;

                List<clsDelincuency> delincuencies = new List<clsDelincuency>();

                string sqlQry = "SELECT tbl_CustomerID.ID AS ID, tbl_CustomerID.CustomerID AS CustomerName, TicketDate, TotalPrice FROM tbl_Tickets " +
                                "INNER JOIN tbl_CustomerID ON tbl_Tickets.CustomerID = tbl_CustomerID.ID " +
                                "WHERE TicketDate LIKE '" + dt + "' AND Status = 1 " +
                                "ORDER BY CustomerName, TicketDate ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            if (firstRec)
                            {
                                custID = Convert.ToInt32(sdr["ID"]);
                                custName = sdr["CustomerName"].ToString();
                                firstRec = false;
                            }

                            if (custID != Convert.ToInt32(sdr["ID"]))
                            {
                                // add record to the list
                                clsDelincuency Delincuency = new clsDelincuency();

                                Delincuency.ID = custID;
                                Delincuency.CustomerName = custName;
                                Delincuency.sum_0_8_days = sum_0_8_days;
                                Delincuency.sum_9_15_days = sum_9_15_days;
                                Delincuency.sum_16_30_days = sum_16_30_days;
                                Delincuency.sum_31_45_days = sum_31_45_days;
                                Delincuency.sum_46_60_days = sum_46_60_days;
                                Delincuency.sum_61_days = sum_61_days;

                                delincuencies.Add(Delincuency);

                                custID = Convert.ToInt32(sdr["ID"]);
                                custName = sdr["CustomerName"].ToString();

                                sum_0_8_days = 0;
                                sum_9_15_days = 0;
                                sum_16_30_days = 0;
                                sum_31_45_days = 0;
                                sum_46_60_days = 0;
                                sum_61_days = 0;
                            }

                            string tickDate = sdr["TicketDate"].ToString();

                            DateTime date1 = new DateTime(Convert.ToInt32(tickDate.Substring(0,4)),
                                                          Convert.ToInt32(tickDate.Substring(4,2)),
                                                          Convert.ToInt32(tickDate.Substring(6, 2)), 0, 0, 0);
                            DateTime date2 = DateTime.Now;

                            double diff = (date2 - date1).TotalDays;

                            if (diff < 9)
                                sum_0_8_days += Convert.ToInt32(sdr["TotalPrice"]);     // 1 - 8 days
                            else if (diff >= 9 && diff <= 15)
                                sum_9_15_days += Convert.ToInt32(sdr["TotalPrice"]);    // 9 - 15 days
                            else if (diff >= 16 && diff <= 30)
                                sum_16_30_days += Convert.ToInt32(sdr["TotalPrice"]);   // 16 - 30 days
                            else if (diff >= 31 && diff <= 45)
                                sum_31_45_days += Convert.ToInt32(sdr["TotalPrice"]);   // 31 - 45 days
                            else if (diff >= 46 && diff <= 60)
                                sum_46_60_days += Convert.ToInt32(sdr["TotalPrice"]);   // 46 - 60 days
                            else
                                sum_61_days += Convert.ToInt32(sdr["TotalPrice"]);      // +60 days
                        }
                        // add last record to the list
                        clsDelincuency Delin = new clsDelincuency();

                        Delin.ID = custID;
                        Delin.CustomerName = custName;
                        Delin.sum_0_8_days = sum_0_8_days;
                        Delin.sum_9_15_days = sum_9_15_days;
                        Delin.sum_16_30_days = sum_16_30_days;
                        Delin.sum_31_45_days = sum_31_45_days;
                        Delin.sum_46_60_days = sum_46_60_days;
                        Delin.sum_61_days = sum_61_days;

                        delincuencies.Add(Delin);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return delincuencies;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<string> GetCustomerListFromtDailyClosing()
        {
            List<string> customerList = new List<string>();
            List<clsCustomerVIP> vipList = new List<clsCustomerVIP>();

            try
            {
                string sqlQry = "SELECT DISTINCT tbl_CustomerID.CustomerID, tbl_CustomerID.Type FROM tbl_CustomerID " +
                                "INNER JOIN tbl_DailyClosing ON tbl_CustomerID.ID = tbl_DailyClosing.CustomerID " +
                                "ORDER BY tbl_CustomerID.CustomerID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            customerList.Add(sdr.GetString(0));

                            clsCustomerVIP customerVIP = new clsCustomerVIP();
                            customerVIP.CustomerID = sdr["CustomerID"].ToString();

                            switch (Convert.ToInt32(sdr["Type"]))
                            {
                                case 1:
                                    customerVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\people.ico";
                                    break;
                                case 2:
                                    customerVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\tables.png";
                                    break;
                            }

                            vipList.Add(customerVIP);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return customerList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsCustomerVIP> GetCustomerListFromtDailyClosing2()
        {
            List<clsCustomerVIP> vipList = new List<clsCustomerVIP>();

            try
            {
                string sqlQry = "SELECT DISTINCT tbl_CustomerID.CustomerID, tbl_CustomerID.Type FROM tbl_CustomerID INNER JOIN tbl_DailyClosing ON tbl_CustomerID.ID = tbl_DailyClosing.CustomerID " +
                                "WHERE tbl_CustomerID.ID IN (SELECT DISTINCT a.CustomerID FROM tbl_DailyClosing A INNER JOIN tbl_Tickets b ON a.CustomerID = b.CustomerID WHERE b.Status = 1) " +
                                "ORDER BY tbl_CustomerID.CustomerID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCustomerVIP customerVIP = new clsCustomerVIP();
                            customerVIP.CustomerID = sdr["CustomerID"].ToString();

                            switch (Convert.ToInt32(sdr["Type"]))
                            {
                                case 1:
                                    customerVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\icons8-tarjeta-de-membresia-94.png";
                                    break;
                                case 2:
                                    customerVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\tables.png";
                                    break;
                                case 3:
                                    customerVIP.ImagePath = @"C:\AWC.DigitalCommerce\Images\damage.png";
                                    break;
                            }
                            vipList.Add(customerVIP);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return vipList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsServiceFeeByWho> GetServiceFeeByWho(string startDate, string finalDate, bool onlyMeals)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsServiceFeeByWho> serviceFeeByWhoList = new List<clsServiceFeeByWho>();
                
                if (onlyMeals)
                {
                    sqlQry = "SELECT tbl_Users.userName AS 'UserName', SUM(ServiceFee) AS 'TotalServiceFee' FROM tbl_Tickets " +
                            $"INNER JOIN tbl_Users ON tbl_Tickets.WhoOpened = tbl_Users.userPIN WHERE TicketDate >= '{startDate}' AND TicketDate <= '{finalDate}' AND Status = 0 " +
                             "GROUP BY UserName";
                }
                else
                {
                    sqlQry = "SELECT tbl_Users.userName AS 'UserName', SUM(ServiceFee) AS 'TotalServiceFee' FROM tbl_Tickets " +
                            $"INNER JOIN tbl_Users ON tbl_Tickets.WhoOpened = tbl_Users.userPIN WHERE TicketDate >= '{startDate}' AND TicketDate <= '{finalDate}' AND Status = 0 " +
                             "GROUP BY UserName";
                }
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsServiceFeeByWho serviceFeeByWho = new clsServiceFeeByWho();
                            serviceFeeByWho.UserName = sdr["UserName"].ToString();
                            serviceFeeByWho.TotalServiceFee = Convert.ToInt32(sdr["TotalServiceFee"]);
                            serviceFeeByWhoList.Add(serviceFeeByWho);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return serviceFeeByWhoList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsProvider> GetProvidersListByItemsSold(List<clsItem> itemsList)
        {
            try
            {
                string itemsIDList = string.Empty;

                itemsIDList = "(";

                foreach (clsItem item in itemsList)
                {
                    itemsIDList += item.ID.ToString() + ",";
                }

                itemsIDList = itemsIDList.Substring(0, itemsIDList.Length - 1) + ")";

                List<clsProvider> providersList = new List<clsProvider>();

                string sqlQry = "SELECT * FROM tbl_Providers WHERE ID IN (SELECT DISTINCT tbl_Invoices.ProviderID FROM tbl_InvoicesDetail " +
                                "INNER JOIN tbl_Invoices ON tbl_InvoicesDetail.InvoiceGUID IN " +
                                "(SELECT DISTINCT InvoiceGUID FROM tbl_InvoicesDetail WHERE ItemID IN " + itemsIDList + "))";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsProvider provider = new clsProvider();
                            provider.ID = Convert.ToInt32(sdr["ID"]);
                            provider.ProviderName = sdr["ProviderName"].ToString();
                            provider.BusinessAddress = sdr["BusinessAddress"].ToString();
                            provider.eMailAddress = sdr["eMailAddress"].ToString();
                            provider.PaymentMethod = sdr["PaymentMethod"].ToString();
                            provider.PhoneNumber = sdr["PhoneNumber"].ToString();
                            provider.CellularNumber = sdr["CellularNumber"].ToString();
                            provider.Remarks = sdr["Remarks"].ToString();
                            providersList.Add(provider);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return providersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsServiceFeeByWho> GetServiceFeeByWhoAndJustMeals(string startDate, string finalDate)
        {
            try
            {
                List<clsServiceFeeByWho> serviceFeeByWhoAndJustMeals = new List<clsServiceFeeByWho>();

                string sqlQry = "";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {

                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return serviceFeeByWhoAndJustMeals;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDetailForDatagrid> GetProductsMostOrdered(int numProducts)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = $"SELECT TOP {numProducts} COUNT(ItemID) AS 'Qty', ItemID FROM tbl_TicketsDetail GROUP BY ItemID ORDER BY COUNT(ItemID) DESC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static int GetOldTicketsCancelled(string workDay)
        {
            try
            {
                int totalOldTicketsPay = 0;

                string sqlQry = $"SELECT * FROM tbl_TicketsOldCancelled WHERE Shift = {Settings.Default.ShiftForQuery} AND PayDate = '{workDay}' ORDER BY TicketID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            totalOldTicketsPay += Convert.ToInt32(sdr["TotalPrice"]);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return totalOldTicketsPay;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static List<clsTicket> GetUncollectibleAccount(string fromDate)
        {
            try
            {
                List<clsTicket> unAccList = new List<clsTicket>();

                string sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate < '{fromDate}' ORDER BY TicketDate, ID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicket unAcc = new clsTicket();
                            unAcc.ID = Convert.ToInt32(sdr["ID"]);
                            unAcc.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            unAcc.CustomerAKA = sdr["customerAKA"].ToString();
                            unAcc.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            unAccList.Add(unAcc);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return unAccList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicket> GetTicketsSummary(string startDate, string endDate, int option)
        {
            try
            {
                List<clsTicket> ticketsSummary = new List<clsTicket>();

                string sqlQry = string.Empty;

                switch (option)
                {
                    // Cash
                    case 0:
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' AND Cash > 0 ORDER BY TicketDate, ID";
                        break;
                    // Credit Card
                    case 1:
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' AND CreditCard > 0 ORDER BY TicketDate, ID";
                        break;
                    // Transfer
                    case 2:
                        sqlQry = $"SELECT * FROM tbl_Tickets WHERE TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' AND Transfer > 0 ORDER BY TicketDate, ID";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                       while (sdr.Read())
                        {
                            clsTicket tck = new clsTicket();
                            tck.ID = Convert.ToInt32(sdr["ID"]);
                            tck.TicketDate = ConverTicketDate(sdr["TicketDate"].ToString());
                            tck.CustomerAKA = sdr["customerAKA"].ToString();

                            switch(option)
                            {
                                // Cash
                                case 0:
                                    tck.TotalPrice = Convert.ToInt32(sdr["Cash"]);
                                    break;
                                // Credit Card
                                case 1:
                                    tck.TotalPrice = Convert.ToInt32(sdr["CreditCard"]);
                                    break;
                                // Transfer
                                case 2:
                                    tck.TotalPrice = Convert.ToInt32(sdr["Transfer"]);
                                    break;
                            }
                            ticketsSummary.Add(tck);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticketsSummary;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsDailyClosing> GetDailyClosingSummary(string startDate, string endDate)
        {
            try
            {
                List<clsDailyClosing> dcsList = new List<clsDailyClosing>();

                string sqlQry = $"SELECT * FROM tbl_DailyClosingSummary WHERE BusinessDate >= '{startDate}' AND BusinessDate <= '{endDate}' ORDER BY BusinessDate ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsDailyClosing dcs = new clsDailyClosing();
                            dcs.BusinessDate            = ConverTicketDate(sdr["BusinessDate"].ToString());
                            dcs.InitialCash             = Convert.ToInt32(sdr["InitialCash"]);
                            dcs.Cash                    = Convert.ToInt32(sdr["Cash"]);
                            dcs.CashByOperator          = Convert.ToInt32(sdr["CashByOperator"]);
                            dcs.CreditCard              = Convert.ToInt32(sdr["CreditCard"]);
                            dcs.CreditCardByOperator    = Convert.ToInt32(sdr["CreditCardByOperator"]);
                            dcs.Transfer                = Convert.ToInt32(sdr["Transfer"]);
                            dcs.TransferByOperator      = Convert.ToInt32(sdr["TransferByOperator"]);
                            dcs.AccountsReceivable      = Convert.ToInt32(sdr["AccountsReceivable"]);
                            dcs.ServiceFee              = Convert.ToInt32(sdr["ServiceFee"]);
                            dcs.GrossSale               = Convert.ToInt32(sdr["GrossSale"]);
                            dcs.NetSale                 = Convert.ToInt32(sdr["NetSale"]);
                            dcs.TotalCashInDrawer       = Convert.ToInt32(sdr["TotalCashInDrawer"]);
                            dcs.DailyClosingMatch       = Convert.ToBoolean(sdr["DailyClosingMatch"]);
                            dcs.WhoDidIt                = sdr["WhoDidIt"].ToString();
                            dcs.CreatedAt               = Convert.ToDateTime(sdr["CreatedAt"]);
                            dcsList.Add(dcs);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return dcsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static int GetInitialCashFromDailyClosingSummary(string workday)
        {
            try
            {
                int InitialCash = 0;

                string sqlQry = $"SELECT TOP 1 InitialCash FROM tbl_DailyClosingSummary WHERE BusinessDate = '{workday}' ORDER BY CreatedAt DESC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            InitialCash += Convert.ToInt32(sdr["InitialCash"]);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return InitialCash;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static List<clsItemDetailForDatagrid> GetItemsInheritedByGUID(string GUID)
        {
            try
            {
                List<clsItemDetailForDatagrid> TicketItems = new List<clsItemDetailForDatagrid>();

                string sqlQry = $"SELECT * FROM tbl_TicketsInheritedDetail WHERE GUID = '{GUID}' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDetailForDatagrid detailItem = new clsItemDetailForDatagrid();

                            detailItem.ID = Convert.ToInt32(sdr["ID"]);
                            detailItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            detailItem.ItemDesc = GetItemDescriptionByItemID(detailItem.ItemID);
                            detailItem.Qty = Convert.ToInt32(sdr["Qty"]);
                            detailItem.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            detailItem.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);

                            TicketItems.Add(detailItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TicketItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> GetMealRelationships(int itemFrom)
        {
            try
            {
                List<clsItem> ItemFromList = new List<clsItem>();

                string sqlQry = $"SELECT * FROM tbl_MealsRelationships WHERE ItemFrom = {itemFrom}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();

                            item.ID = Convert.ToInt32(sdr["ItemTo"]);
                            item.ItemAvailable = Convert.ToInt32(sdr["Qty"]);

                            ItemFromList.Add(item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ItemFromList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<ATVResponse> GetIVAInvoices(string startDate, string endDate)
        {
            try
            {
                List<ATVResponse> ATVTicketsList = new List<ATVResponse>();

                string sqlQry = $"SELECT * FROM tbl_Tickets WHERE ATVStatusCode = 200 AND TicketDate >= '{startDate}' AND TicketDate <= '{endDate}' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            ATVResponse ATVTicket = new ATVResponse();

                            ATVTicket.ID = Convert.ToInt32(sdr["ID"]);
                            ATVTicket.internal_id = Convert.ToInt32(sdr["ATVInternalID"]);
                            ATVTicket.consecutivo = sdr["ATVConsecutive"].ToString();
                            ATVTicket.clave = sdr["ATVKey"].ToString();
                            ATVTicket.estado = sdr["ATVStateMsj"].ToString();
                            
                            ATVTicketsList.Add(ATVTicket);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ATVTicketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static List<clsBucketsDetail> GetBucketsByTicketNumber(int ticketID)
        {
            try
            {
                List<clsBucketsDetail> bucketsDetail = new List<clsBucketsDetail>();

                string sqlQry = $"SELECT * FROM tbl_BucketsDetail WHERE TicketNumber = {ticketID} ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsBucketsDetail bucketItem = new clsBucketsDetail();

                            bucketItem.ID = Convert.ToInt32(sdr["ID"]);
                            bucketItem.GUID = sdr["GUID"].ToString();
                            bucketItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            bucketItem.Qty = Convert.ToInt32(sdr["Qty"]);

                            bucketsDetail.Add(bucketItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return bucketsDetail;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static List<clsCashIncomes> GetIncomeCash(string startDate, string endDate)
        {
            try
            {
                List<clsCashIncomes> cashIncomesList = new List<clsCashIncomes>();

                string sqlQry = $"SELECT * FROM tbl_CashIncomes WHERE BusinessDate >= '{startDate}' AND BusinessDate <= '{endDate}' AND Shift = {Settings.Default.Shift} ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCashIncomes incomeCash = new clsCashIncomes();

                            incomeCash.ID = Convert.ToInt32(sdr["ID"]);
                            incomeCash.BusinessDate = ConverTicketDate(sdr["BusinessDate"].ToString());
                            incomeCash.IncomeDescription = sdr["IncomeDescription"].ToString();
                            incomeCash.IncomeAmount = Convert.ToInt32(sdr["IncomeAmount"]);

                            clsUser userProf = CheckUserPIN(sdr["WhoDidIt"].ToString());
                            incomeCash.WhoDidIt = userProf.userName;
                            incomeCash.CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]);

                            cashIncomesList.Add(incomeCash);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return cashIncomesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTicketProform> CheckTicketProforms(int ticketNumber, string customerAKA)
        {
            try
            {
                List<clsTicketProform> proForms = new List<clsTicketProform>();

                string sqlQry = string.Empty;

                if (customerAKA.Length == 0)
                {
                    sqlQry = $"SELECT * FROM tbl_TicketsProforms WHERE TicketNumber = {ticketNumber}";
                }
                else
                {
                    sqlQry = $"SELECT * FROM tbl_TicketsProforms WHERE TicketNumber = {ticketNumber} AND CustomerAKA = '{customerAKA}'";
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketProform proform = new clsTicketProform();

                            proform.ID = Convert.ToInt32(sdr["ID"]);
                            proform.TicketNumber = Convert.ToInt32(sdr["TicketNumber"]);
                            proform.TicketDetailID = Convert.ToInt32(sdr["TicketDetailID"]);
                            proform.CustomerAKA = sdr["CustomerAKA"].ToString();
                            proform.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            proform.Qty = Convert.ToInt32(sdr["Qty"]);

                            proForms.Add(proform);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return proForms;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<string> GetTicketProformsCustomerAKAList(int ticketNumber)
        {
            try
            {
                List<string> customersAKA = new List<string>();

                string sqlQry = $"SELECT DISTINCT CustomerAKA FROM tbl_TicketsProforms WHERE TicketNumber = {ticketNumber} ORDER BY CustomerAKA ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            customersAKA.Add(sdr["CustomerAKA"].ToString());
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return customersAKA;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsTicketProform GetTicketProformByTicketDetailID(int ticketDetailID)
        {
            try
            {
                clsTicketProform ticketProformItem = new clsTicketProform();

                string sqlQry = $"SELECT * FROM tbl_TicketsProforms WHERE TicketDetailID = {ticketDetailID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticketProformItem.ID = Convert.ToInt32(sdr["ID"]);
                        ticketProformItem.TicketNumber = Convert.ToInt32(sdr["TicketNumber"]);
                        ticketProformItem.TicketDetailID = Convert.ToInt32(sdr["TicketDetailID"]);
                        ticketProformItem.CustomerAKA = sdr["CustomerAKA"].ToString();
                        ticketProformItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                        ticketProformItem.Qty = Convert.ToInt32(sdr["Qty"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticketProformItem;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsTimeCard> GetTimeCards(string startDate, string endDate)
        {
            try
            {
                List<clsTimeCard> TimeCardsList = new List<clsTimeCard>();

                string sqlQry = $"SELECT * FROM tbl_Timecards WHERE BusinessDate >= '{startDate}' AND BusinessDate <= '{endDate}' ORDER BY EventDatetime ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTimeCard timeCard = new clsTimeCard();

                            timeCard.BusinessDate = sdr["BusinessDate"].ToString();
                            timeCard.UserPIN = Convert.ToInt32(sdr["UserPIN"]);
                            timeCard.EventType = Convert.ToInt32(sdr["EventType"]);
                            timeCard.EventDatetime = Convert.ToDateTime(sdr["EventDatetime"]);

                            TimeCardsList.Add(timeCard);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TimeCardsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static List<clsTimeCard> GetTimeCards(string businessDate)
        {
            try
            {
                List<clsTimeCard> TimeCardsList = new List<clsTimeCard>();

                string sqlQry = $"SELECT * FROM tbl_Timecards WHERE BusinessDate = '{businessDate}' ORDER BY EventDatetime ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTimeCard timeCard = new clsTimeCard();

                            timeCard.BusinessDate = sdr["BusinessDate"].ToString();
                            timeCard.UserPIN = Convert.ToInt32(sdr["UserPIN"]);
                            timeCard.EventType = Convert.ToInt32(sdr["EventType"]);
                            timeCard.EventDatetime = Convert.ToDateTime(sdr["EventDatetime"]);

                            TimeCardsList.Add(timeCard);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return TimeCardsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemsChangePrice> GetItemsChangePrice(string businessDate)
        {
            try
            {
                List<clsItemsChangePrice> ItemsChangePriceList = new List<clsItemsChangePrice>();

                string sqlQry = $"SELECT * FROM tbl_ItemsChangePrice WHERE BusinessDate = '{businessDate}' ORDER BY MadeItAt ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemsChangePrice item = new clsItemsChangePrice();

                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.BusinessDate = sdr["BusinessDate"].ToString();
                            item.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            item.PreviousPrice = Convert.ToInt32(sdr["PreviousPrice"]);
                            item.CurrentPrice = Convert.ToInt32(sdr["CurrentPrice"]);
                            item.WhoDidit = sdr["WhoDidit"].ToString();
                            item.MadeItAt = Convert.ToDateTime(sdr["MadeItAt"]);

                            ItemsChangePriceList.Add(item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ItemsChangePriceList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsOpenCashDrawer> GetOpenCashDrawer(string startDate, string endDate)
        {
            try
            {
                List<clsOpenCashDrawer> openCashDrawerList = new List<clsOpenCashDrawer>();

                string sqlQry = $"SELECT * FROM tbl_MoneyDrawerLog WHERE BusinessDate >= '{startDate}' AND BusinessDate <= '{endDate}' ORDER BY EventDateTime ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsOpenCashDrawer openCashDrawer = new clsOpenCashDrawer();

                            openCashDrawer.EventDateTime = Convert.ToDateTime(sdr["EventDatetime"]);
                            openCashDrawer.WhoDitIt = sdr["WhoDidIt"].ToString();

                            openCashDrawerList.Add(openCashDrawer);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return openCashDrawerList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static List<clsPayMethodChange> GetPayMethodChanges(string businessDate)
        {
            try
            {
                List<clsPayMethodChange> pmcl = new List<clsPayMethodChange>();

                string sqlQry = $"SELECT * FROM tbl_PayMethodChange WHERE TicketDate = '{businessDate}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsPayMethodChange pmc = new clsPayMethodChange();

                            pmc.ID = Convert.ToInt32(sdr["ID"]);
                            pmc.TicketDate = sdr["TicketDate"].ToString();
                            pmc.TicketID = Convert.ToInt32(sdr["TicketID"]);

                            pmc.OrigCash = Convert.ToInt32(sdr["OrigCash"]);
                            pmc.OrigCreditCard = Convert.ToInt32(sdr["OrigCreditCard"]);
                            pmc.OrigTransfer = Convert.ToInt32(sdr["OrigTransfer"]);

                            pmc.CurrCash = Convert.ToInt32(sdr["CurrCash"]);
                            pmc.CurrCreditCard = Convert.ToInt32(sdr["CurrCreditCard"]);
                            pmc.CurrTransfer = Convert.ToInt32(sdr["CurrTransfer"]);

                            pmc.WhoDidIt = sdr["WhoDidIt"].ToString();
                            pmc.MadeItAt = Convert.ToDateTime(sdr["MadeItAt"]);

                            pmcl.Add(pmc);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return pmcl;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

        }
        public static int GetCashOnHandAtTheBeginning()
        {
            try
            {
                int getCashOnHandAtTheBeginning = 0;

                string sqlQry = $"SELECT * FROM tbl_CustomerID WHERE CustomerID = 'AWCDIGITALCOMMERCE'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        getCashOnHandAtTheBeginning = Convert.ToInt32(sdr["CreditLimit"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return getCashOnHandAtTheBeginning;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }

        }
        public static List<clsVoucher> GetVouchers(string businessDate)
        {
            try
            {
                List<clsVoucher> VouchersList = new List<clsVoucher>();

                string sqlQry = $"SELECT * FROM tbl_Vouchers WHERE BusinessDate = '{businessDate}' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsVoucher voucher = new clsVoucher();

                            voucher.ID = Convert.ToInt32(sdr["ID"]);
                            voucher.BusinessDate = sdr["BusinessDate"].ToString();
                            voucher.IssueBy = sdr["IssueBy"].ToString();
                            voucher.Amount = Convert.ToInt32(sdr["Amount"]);
                            voucher.CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]);
                            voucher.ExpireAt = sdr["ExpireAt"].ToString();

                            VouchersList.Add(voucher);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return VouchersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsOpenDrawerRequest> GetOpenDrawerRequest(string businessDate)
        {
            try
            {
                List<clsOpenDrawerRequest> odrList = new List<clsOpenDrawerRequest>();

                string sqlQry = $"SELECT * FROM tbl_OpenCashDrawerRequest WHERE BusinessDate = '{businessDate}' ORDER BY ID ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsOpenDrawerRequest odr = new clsOpenDrawerRequest();

                            odr.ID = Convert.ToInt32(sdr["ID"]);
                            odr.BusinessDate = sdr["BusinessDate"].ToString();
                            odr.WhoOpen = Convert.ToInt32(sdr["WhoOpen"]);
                            odr.CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]);

                            odrList.Add(odr);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return odrList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region INSERT
        public static int InsertNewTicket(clsTicket newTicket, int whoOpened)
        {
            try
            {
                int tnum = 0;
                int applyServiceFee = newTicket.ApplyServiceFee ? 1 : 0;

                string sqlQry = "INSERT INTO tbl_Tickets(TicketDate, GUID, CustomerID, TotalPrice, PayMethod, Status, WhoOpened, WhoClosed, Splited, ApplyServiceFee, CustomerAKA, Shift) OUTPUT INSERTED.ID " +
                               $"VALUES ('{newTicket.TicketDate}', '{newTicket.GUID}', {newTicket.CustID}, {newTicket.TotalPrice}, 0, 1, {whoOpened}, 0, 0, {applyServiceFee}, '{newTicket.CustomerAKA}', {newTicket.Shift})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        tnum = Convert.ToInt32(sdr["ID"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                UpdateCustomerStatus(newTicket.CustID, 1);
                return tnum;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static bool InsertNewTicketAborted(int ticketAborted)
        {
            try
            {
                clsTicket tmp = GetTicket(ticketAborted);

                string sqlQry = "INSERT INTO tbl_TicketsAborted SELECT * FROM tbl_Tickets WHERE ID = " + ticketAborted + " " +
                                "INSERT INTO tbl_TicketsDetailAborted SELECT * FROM tbl_TicketsDetail WHERE GUID = '" + tmp.GUID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool IncludeAbortReason(int ticketAborted, string abortReason, int whoApproved)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET CloseAt = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', AbortReason = '{abortReason}', WhoClosed = {whoApproved} WHERE ID = {ticketAborted}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertTicketDetail(List<clsTicketDetail> TicketDetail, string guidID, int whoUpdated, bool UpdateTicket)
        {
            try
            {
                bool result = false;
                int totAmount = 0;
                string dt = Settings.Default.BusinessDate;
                string sqlQry = string.Empty;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();

                    foreach (clsTicketDetail tckdet in TicketDetail)
                    {
                        for (int i = 1; i <= tckdet.Qty; i++)
                        {
                            sqlQry = "INSERT INTO tbl_TicketsDetail (GUID, Qty, ItemID, UnitPrice, UnitCost, TotalPrice, TotalCost, WhoUpdated, Splited, CreatedAt, Remarks) " +
                                     $"VALUES('{guidID}', 1, {tckdet.ItemID}, {tckdet.UnitPrice}, {tckdet.UnitCost}, {tckdet.UnitPrice}, {tckdet.UnitCost}, {whoUpdated}, 0, '{dt}', '{tckdet.Note}')";

                            SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                            sqlCmd.ExecuteNonQuery();
                            totAmount += tckdet.UnitPrice;

                            if (Settings.Default.DebugTrace)
                                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                        }
                    }
                    result = true;
                }

                if (UpdateTicket)
                    result = UpdateTicketTotalPrice(guidID, totAmount);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static int CreateNextTicket(string GUID, int internalCustID)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Tickets(TicketDate, GUID, CustomerID, TotalPrice, PayMethod, Status, WhoOpened, WhoClosed, Splited, Shift) " +
                                $"VALUES ('{Settings.Default.BusinessDate}', '{GUID}', {internalCustID}, 0, 1, 1, {Settings.Default.WhoOpen}, {Settings.Default.WhoOpen}, 0, {Settings.Default.Shift})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return GetTempTicket(GUID);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        private static int GetTempTicket(string QuickOrderGUID)
        {
            try
            {
                int tickectNumber = 0;

                string sqlQry = "SELECT ID FROM tbl_Tickets WHERE GUID = '" + QuickOrderGUID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        tickectNumber = Convert.ToInt32(sdr["ID"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return tickectNumber;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        public static bool InsertNewItem(int itemType, string itemDesc, int itemPrice, int itemCost, int itemUofM, int itemUN, bool IsActive, int itemSubtype)
        {
            try
            {
                int ia = IsActive == true ? 1 : 0;

                string sqlQry = "INSERT INTO tbl_Items (ItemType, ItemSubType, ItemDescription, IsActive, UnitPrice, UnitCost, ItemUnitOfMeasurement, ItemUnitSize) " + "" +
                                $"VALUES ({itemType}, {itemSubtype}, '{itemDesc}', {ia},  {itemPrice}, {itemCost}, {itemUofM}, {itemUN})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertNewCustomer(string custID, int type, int subType, int status, int serviceFee, int freeOfcharge, int creditLimit)
        {
            try
            {
                bool result = false;

                string sqlQry = "INSERT INTO tbl_CustomerID (Type, SubType, CustomerID, LastPayment, Active, ApplyServiceFee, FreeOfCharge, CreditLimit) " +
                                $"VALUES ({type}, {subType}, '{custID.ToUpper()}', '{Settings.Default.BusinessDate}', {status}, {serviceFee}, {freeOfcharge}, {creditLimit})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool InsertNewTableSeat(string custID)
        {
            try
            {
                bool result = false;
                int applyService = Settings.Default.ApplyServiceFee ? 1 : 0;

                string sqlQry = "INSERT INTO tbl_CustomerID (Type, SubType, CustomerID, LastPayment, Active, ApplyServiceFee, FreeOfCharge, CreditLimit) " +
                                $"VALUES (2, 0, '{custID}', '{Settings.Default.BusinessDate}', 0, {applyService}, 0, 0)";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool InsertNewOpenTicket(clsCustomerVIP custProfile)
        {
            try
            {
                bool result = false;

                int afs = custProfile.ApplyServiceFee ? 1 : 0;
                int foc = custProfile.CustomerFOC ? 1 : 0;

                string sqlQry = "INSERT INTO tbl_OpenTickets (ID, Type, CustomerID, Active, LastPayment, ApplyServiceFee, FreeOfCharge) " +
                                "VALUES (" + custProfile.ID + ", " + custProfile.Type + ", '" + custProfile.CustomerID.ToUpper() + "', 1, '" + Settings.Default.BusinessDate + "', " + afs + ", " + foc + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool InsertUserSecurityProfile(clsUser userProf)
        {
            try
            {
                bool result = false;
                int active = userProf.userActive ? 1 : 0;
                int powerAdmin = userProf.userPowerAdmin ? 1 : 0;

                string sqlQry = "INSERT INTO tbl_Users (userPIN, userPW, userName,  userAccessLevel, userActive, userSecurityProfile, userPowerAdmin) " +
                                "VALUES ('" + userProf.userPIN + "', '" +
                                              userProf.userPW + "', '" +
                                              userProf.userName + "', '" +
                                              userProf.userAccessLevel + "', " +
                                              active + ", '" +
                                              userProf.userSecurityProfile + "', " + powerAdmin + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool InsertLunch(clsLunch lunch)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Lunches (LunchDate, GUID, EmployeeName, Qty, MealID) " + 
                                "VALUES ('" + lunch.LunchDate + "', '" + lunch.GUID + "', '" + lunch.EmployeeName + "', " + lunch.Qty + ", " + lunch.MealID + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertPayment(clsSmallPayment smlPay)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Payments (PaymentDate, RandomRef, CustomerID, TicketID, CurTotalPrice, PaymentAmount, Cash, CreditCard, Transfer, NewTotalPrice, WhoClosed) " +
                                "VALUES ('" + Settings.Default.BusinessDate + "', '" +
                                smlPay.RandomRef + "', " +
                                smlPay.CustomerID + ", " +
                                smlPay.TicketID + ", " +
                                smlPay.CurTotalPrice + ", " +
                                smlPay.PaymentAmount + ", " +
                                smlPay.Cash + ", " +
                                smlPay.CreditCard + ", " +
                                smlPay.Transfer + ", " +
                                smlPay.NewTotalPrice + ", " +
                                smlPay.WhoClosed + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();

                    sqlQry = "UPDATE tbl_Tickets SET Payments = Payments + " + smlPay.PaymentAmount +
                             ", Cash = Cash + " + smlPay.Cash +
                             ", CreditCard =  CreditCard + " + smlPay.CreditCard +
                             ", Transfer = Transfer + " + smlPay.Transfer + " WHERE ID = " + smlPay.TicketID;
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertTicketModified(clsTicketModified t)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_TicketsModified (ID, origTicketDate, origCustomerID, origTotalPrice, origServiceFee, origPayments, origCash, origCreditCard, origTransfer, origCreatedAt, " +
                                "modTicketDate, modCustomerID, modTotalPrice, modServiceFee, modPayments, modCash, modCreditCard, modTransfer, modCreatedAt) " +
                                "VALUES (" + t.ID + ", '" + t.oriTicketDate + "', " + t.oriCustID + ", " + t.oriTotalPrice + ", " + t.oriServiceFee + ", " + t.oriPayments + ", " + t.oriCash + ", " + t.oriCreditCard + ", " + t.oriTransfer + ", '" + t.oriCreateAt + "', " +
                                                      "'" + t.modTicketDate + "', " + t.modCustID + ", " + t.modTotalPrice + ", " + t.modServiceFee + ", " + t.modPayments + ", " + t.modCash + ", " + t.modCreditCard + ", " + t.modTransfer + ", GETDATE())";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertDailyClosingSummary(clsDailyClosing dcs)
        {
            try
            {
                int dailyClosingMatch = dcs.DailyClosingMatch ? 1 : 0;

                string sqlQry = $"DELETE tbl_DailyClosingSummary WHERE BusinessDate = {dcs.BusinessDate} AND Shift = {dcs.Shift}; INSERT INTO tbl_DailyClosingSummary (BusinessDate, Shift, InitialCash, IncomeCash, Cash, CashByOperator, CreditCard, CreditCardByOperator, Transfer, TransferByOperator, AccountsReceivable, ServiceFee, GeneralExpenses, GrossSale, NetSale, TotalCashInDrawer, DailyClosingMatch, WhoDidIt) " +
                                $"VALUES ('{dcs.BusinessDate}', {dcs.Shift}, {dcs.InitialCash}, {dcs.IncomeCash}, {dcs.Cash}, {dcs.CashByOperator},{dcs.CreditCard}, {dcs.CreditCardByOperator}, {dcs.Transfer}, {dcs.TransferByOperator}, {dcs.AccountsReceivable}, {dcs.ServiceFee}, {dcs.Expenses}, {dcs.GrossSale}, {dcs.NetSale}, {dcs.TotalCashInDrawer}, {dailyClosingMatch}, '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertOldTicketCancelled(string payDate, int ticketID, int TotalPrice)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_TicketsOldCancelled (PayDate, TicketID, TotalPrice, Splited) VALUES ('{payDate}', {ticketID}, {TotalPrice}, {Settings.Default.Shift})";
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertTimecard(string userPIN, bool eventType)
        {
            try
            {
                int et = eventType ? 1 : 0;

                string sqlQry = $"INSERT INTO tbl_Timecards (BusinessDate, userPIN, EventType) VALUES ('{Settings.Default.BusinessDate}', '{userPIN}', {et})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InserttTicketReassigned(int ticketID, string oldID, string newID)
        {
            try
            {
                clsUser user = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());

                string sqlQry = $"INSERT INTO tbl_TicketsReassigned (TicketDate, TicketID, FromCustomer, ToCustomer, WhoMakeit) VALUES ('{Settings.Default.BusinessDate}', {ticketID}, '{oldID}', '{newID}', '{user.userName}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InserttTicketInherited(int ticketID, string fromCustomer, string toCustomer, string GUID)
        {
            try
            {
                clsUser user = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());

                string sqlQry = $"INSERT INTO tbl_TicketsInherited (TicketDate, TicketID, TicketGUID, FromCustomer, ToCustomer, WhoMakeit) VALUES ('{Settings.Default.BusinessDate}', {ticketID}, '{GUID}', '{fromCustomer}', '{toCustomer}', '{user.userName}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertTicketInheritedDetail(string GUID)
        {
            try
            {
                List<clsTicketDetail> itemsDetail = new List<clsTicketDetail>();

                string sqlQry = $"SELECT * FROM tbl_TicketsDetail WHERE GUID = '{GUID}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsTicketDetail itemDetail = new clsTicketDetail();
                            itemDetail.GUID = GUID;
                            itemDetail.Qty = Convert.ToInt32(sdr["Qty"]);
                            itemDetail.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            itemDetail.UnitPrice = Convert.ToInt32(sdr["UnitPrice"]);
                            itemDetail.TotalPrice = Convert.ToInt32(sdr["TotalPrice"]);
                            itemsDetail.Add(itemDetail);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                if (itemsDetail.Count > 0)
                {
                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();

                        foreach (clsTicketDetail itemDetail in itemsDetail)
                        {
                            sqlQry = $"INSERT INTO tbl_TicketsInheritedDetail (GUID, Qty, ItemID, UnitPrice, TotalPrice) VALUES ('{itemDetail.GUID}', {itemDetail.Qty}, {itemDetail.ItemID}, {itemDetail.UnitPrice}, {itemDetail.TotalPrice})";
                            SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertMealOriginDestinyRelation(int itemType, string itemFrom, string itemTo, int qty, int isActive)
        {
            try
            {
                int origID = GetIDByItemDescription(itemFrom);
                int destID = GetIDByItemDescription(itemTo);

                string sqlQry = $"INSERT INTO tbl_MealsRelationships (ItemType, ItemFrom, ItemTo, Qty, IsActive) VALUES({itemType}, {origID}, {destID}, {qty}, {isActive})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertItemDeleted(int ticketID, string itemDescription, int qty, string whoAuth)
        {
            try
            {
                int ID = GetIDByItemDescription(itemDescription);

                string sqlQry = $"INSERT INTO tbl_ItemsDeleted (ID, TicketDate, ItemID, Qty, WhoDeleted, WhoAuth) VALUES ({ticketID}, '{Settings.Default.BusinessDate}', {ID}, {qty}, {Settings.Default.WhoOpen}, '{whoAuth}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertATVTicket(ATVQuery atvqry)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_ATV (TicketID, CustomerName, SSN_Type, SSN, CountryCode, PhoneNumber, eMailAddress) VALUES ({atvqry.TicketID}, '{atvqry.CustomerName}',{atvqry.SSN_Type}, {atvqry.SSN}, {atvqry.CountryCode}, {atvqry.PhoneNumber}, '{atvqry.eMailAddress}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertBucketDetail(int ticket, string GUID, int itemID, int qty)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_BucketsDetail (TicketNumber, GUID, ItemID, Qty) VALUES ({ticket}, '{GUID}', {itemID}, {qty})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

        }
        public static bool InsertIncome(string incomeDescription, int incomeAmount)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_CashIncomes  (BusinessDate, Shift, IncomeDescription, IncomeAmount, WhoDidIt) VALUES ({Settings.Default.BusinessDate}, {Settings.Default.Shift}, '{incomeDescription}', {incomeAmount}, '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertMoneyDrawerLog()
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_MoneyDrawerLog(BusinessDate, WhoDidIt) VALUES ('{Settings.Default.BusinessDate}', '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertInternalOrder(string fileName)
        {
            try
            {
                string sqlQry = string.Empty;

                using (StreamReader sr = new StreamReader(fileName))
                {
                    bool firstRec = true;
                    Guid guidID = Guid.NewGuid();

                    while (!sr.EndOfStream)
                    {
                        string rec = sr.ReadLine();

                        if (firstRec)
                        {
                            sqlQry = $"INSERT INTO tbl_InternalOrders (OrderDate, GUID, OrderDescription, WhoDidIt) VALUES ('{Settings.Default.BusinessDate}', '{guidID.ToString().ToUpper()}', '{rec}', '{Settings.Default.WhoOpen}')";
                            firstRec = false;
                        }
                        else
                        {
                            sqlQry = $"INSERT INTO tbl_InternalOrdersDetail (GUID, ItemDescription, Qty) VALUES ('{guidID.ToString().ToUpper()}', '{rec.Split(',')[0]}', {rec.Split(',')[1]})";
                        }

                        using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                        {
                            sqlConn.Open();
                            sqlCmd = new SqlCommand(sqlQry, sqlConn);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }


                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertTicketProform(int ticketNumber, int ticketDetailID, string customerAKA, int itemID, int qty)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_TicketsProforms (TicketNumber, TicketDetailID, CustomerAKA, ItemID, Qty) VALUES ({ticketNumber}, {ticketDetailID}, '{customerAKA}', {itemID}, {qty})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertItemsChangePrice(int itemID, int previousPrice, int currentPrice)
        {
            try
            {
                if ((previousPrice - currentPrice) == 0) return true;

                string sqlQry = $"INSERT INTO tbl_ItemsChangePrice (BusinessDate, ItemID, PreviousPrice, CurrentPrice, WhoDidit) VALUES ('{Settings.Default.BusinessDate}', {itemID}, {previousPrice}, {currentPrice}, '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertPayMethodChange(clsPayMethodChange pm)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_PayMethodChange (TicketDate, TicketID, OrigCash, OrigCreditCard, OrigTransfer, CurrCash, CurrCreditCard, CurrTransfer, WhoDidIt) VALUES ('{pm.TicketDate}', {pm.TicketID}, {pm.OrigCash}, {pm.OrigCreditCard}, {pm.OrigTransfer}, {pm.CurrCash}, {pm.CurrCreditCard}, {pm.CurrTransfer}, '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertCashOnDrawer(int cash, int withdrawal)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_CashOnDrawer (BusinessDate, Shift, CashAvailable, CashWithdrawal, CashRemaining, WhoDidIt) VALUES ('{Settings.Default.BusinessDate}', {Settings.Default.Shift}, {cash}, {withdrawal}, {cash - withdrawal}, '{Settings.Default.WhoOpen}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static clsVoucher InsertVoucher(int VoucherAmount)
        {
            try
            {
                clsVoucher voucher = new clsVoucher();

                DateTime currentDateTime = DateTime.Now;
                DateTime expirationDateTime = currentDateTime.AddDays(Settings.Default.VoucherExpirationRange);
                string expDate = expirationDateTime.ToString("yyyyMMdd");

                string sqlQry = $"INSERT INTO tbl_Vouchers (BusinessDate, IssueBy, Amount, ExpireAt) VALUES ('{Settings.Default.BusinessDate}', '{Settings.Default.WhoOpen}', {VoucherAmount}, '{expDate}'); SELECT SCOPE_IDENTITY();";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    var newId = sqlCmd.ExecuteScalar();

                    voucher.ID = Convert.ToInt32(newId);
                    voucher.BusinessDate = Settings.Default.BusinessDate;
                    voucher.IssueBy = Settings.Default.WhoOpen.ToString();
                    voucher.Amount = VoucherAmount;
                    voucher.CreatedAt = currentDateTime;
                    voucher.ExpireAt = expirationDateTime.ToString("yyyyMMdd");
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return voucher;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static bool InsertOpenCashDrawerRequest()
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_OpenCashDrawerRequest(BusinessDate, WhoOpen) VALUES ('{Settings.Default.BusinessDate}', {Settings.Default.WhoOpen})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertSalaryAdvance(clsSalaryAdvance salAdv)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_SalaryAdvances (BusinessDate, Amount, Requester, Approver) VALUES ('{salAdv.BusinessDate}', {salAdv.Amount}, '{salAdv.Requester}', {salAdv.Approver})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    var newId = sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region DELETE
        public static bool DeleteCustomer(int ID)
        {
            try
            {
                bool result = false;

                string sqlQry = $"DELETE FROM tbl_CustomerID WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool DeleteUserProfile(string ID)
        {
            try
            {
                bool result = false;

                string sqlQry = "DELETE FROM tbl_Users WHERE userPIN = '" + ID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteOpenTickets(int ID)
        {
            try
            {
                bool result = false;

                string sqlQry = string.Empty;

                if (ID == 0)
                    sqlQry = "TRUNCATE TABLE tbl_OpenTickets";
                else
                    sqlQry = "DELETE FROM tbl_OpenTickets WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool DeleteItem(int ID)
        {
            try
            {
                string sqlQry = "DELETE FROM tbl_Items WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteOriginDestinyRelation(int itemOrig)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Items SET ItemSubType = 0, ItemParent = 0, ItemParentUnit = 0  WHERE ID = " + itemOrig;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteMealOriginDestinyRelation(int idFrom, int idTo)
        {
            try
            {
                string sqlQry = $"DELETE tbl_MealsRelationships WHERE ItemFrom = {idFrom} AND ItemTo = {idTo}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteTicketDetail(string GUID, bool updateTicket)
        {
            try
            {
                bool result = false;

                string sqlQry = "DELETE FROM tbl_TicketsDetail WHERE GUID = '" + GUID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                if (updateTicket)
                    result = UpdateTicketTotalPrice(GUID, 0);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteSplitTicketDetail(clsTicketDetail ticketDetail, bool updateTicket)
        {
            try
            {
                bool result = false;

                string sqlQry = $"DELETE FROM tbl_TicketsDetail WHERE ID = {ticketDetail.ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                if (updateTicket)
                    result = UpdateTicketTotalPrice(ticketDetail.GUID, ticketDetail.TotalPrice * -1);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteOldTicket(int ticketNumber)
        {
            try
            {
                bool result = false;

                string sqlQry = "DELETE FROM tbl_DailyClosing WHERE TicketNumber = " + ticketNumber;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }

        }
        public static bool DeleteExpense(int ID)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_Expenses WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool DeleteHistoryByCustomerID(int custID)
        {
            try
            {
                string guidList = string.Empty;
                string ticketList = string.Empty;
                string sqlQry = string.Empty;

                #region DELETE tbl_Tickets & tbl_TicketsDetail
                sqlQry = $"SELECT ID, GUID FROM tbl_Tickets WHERE CustomerID = {custID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            ticketList += sdr["ID"].ToString() + ",";
                            guidList += "'" + sdr["GUID"].ToString() + "',";
                        }
                        ticketList += "0";
                        guidList += "'dummy'";
                    }
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                // delete all the details of all the tickets
                if (guidList.Length > 0)
                {
                    sqlQry = $"DELETE tbl_TicketsDetail WHERE GUID IN ({guidList})";

                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                }

                // delete all the tickets
                sqlQry = $"DELETE tbl_Tickets WHERE CustomerID = {custID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                #endregion

                #region DELETE tbl_TicketsAborted & tbl_TicketsDetailAborted
                sqlQry = $"SELECT GUID FROM tbl_TicketsAborted WHERE CustomerID = {custID}";

                guidList = string.Empty;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            guidList += "'" + sdr["GUID"].ToString() + "',";
                        }
                        guidList += "'dummy'";
                    }
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                // delete all the details of all the tickets
                if (guidList.Length > 0)
                {
                    sqlQry = $"DELETE tbl_TicketsDetailAborted WHERE GUID IN ({custID})";

                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                }

                // delete all the tickets
                sqlQry = $"DELETE tbl_TicketsAborted WHERE CustomerID = {custID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                #endregion

                #region DELETE tbl_TicketsOldCancelled, tbl_Payments, tbl_OpenTickets
                if (ticketList.Length > 0)
                {
                    sqlQry = $"DELETE tbl_TicketsOldCancelled WHERE TicketID IN ({ticketList})";

                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                    sqlQry = $"DELETE tbl_Payments WHERE CustomerID = {custID}";

                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                    sqlQry = $"DELETE tbl_OpenTickets WHERE CustomerID = {custID}";

                    using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                }
                #endregion

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool DeleteBucketDetailByTicketNumber(int ID)
        {
            try
            {
                bool result = false;

                string sqlQry = $"DELETE FROM tbl_BucketsDetail WHERE TicketNumber = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static bool DeleteTicketProform(int ID, string customerAKA)
        {
            try
            {
                bool result = false;

                string sqlQry = $"DELETE FROM tbl_TicketsProforms WHERE TicketNumber = {ID} AND CustomerAKA = '{customerAKA}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static bool NormalizeDailyClosingTable()
        {
            try
            {
                bool result = false;

                string sqlQry = "DELETE tbl_DailyClosing WHERE TicketNumber IN (SELECT A.TicketNumber FROM tbl_DailyClosing A INNER JOIN tbl_Tickets B ON A.TicketNumber = B.ID WHERE B.Status = 0)";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static bool InsertItemDeletedFromSystem(clsItemDeletedFromSystem idfs)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_ItemsDeletedFromSystem (TicketDate, ItemID, ItemDescription, whoDeleted, whoDeletedName)" +
                                "VALUES ('" + idfs.TicketDate + "', "
                                           + idfs.ItemID + ", '"
                                           + idfs.ItemDescription + "', "
                                           + idfs.WhoDeleted + ", '"
                                           + idfs.WhoDeletedName + "')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InsertItemDeletedFromSystem", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region UPDATES
        public static bool UpdateItem(int ID, int itemType, int itemPrice, int itemCost, bool IsActive, int ItemSubType)
        {
            try
            {
                int ia = IsActive == true ? 1 : 0;

                string sqlQry = $"UPDATE tbl_Items SET ItemType = {itemType}, ItemSubType = {ItemSubType}, UnitPrice = {itemPrice}, UnitCost = {itemCost}, IsActive = {ia} WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateOriginDestinyRelation(string _orig, string _dest, int parentUnit)
        {
            try
            {
                int origID = GetIDByItemDescription(_orig);
                int destID = GetIDByItemDescription(_dest);

                string sqlQry = "UPDATE tbl_Items SET ItemSubType = 1, ItemParent = " + destID + ", ItemParentUnit = " + parentUnit + " WHERE ID = " + origID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateTicketTotalPrice(string guid, int addAmount)
        {
            try
            {
                bool result = false;
                string sqlQry = String.Empty;

                if (addAmount == 0)
                    sqlQry = "UPDATE tbl_Tickets SET TotalPrice = 0 WHERE GUID = '" + guid + "'";
                else
                    sqlQry = "UPDATE tbl_Tickets SET TotalPrice = TotalPrice + " + addAmount + " WHERE GUID = '" + guid + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateTicketCustomerID(int ticketNumber, int customerID, string customerName, bool applyServiceFee)
        {
            try
            {
                bool result = false;
                int asf = applyServiceFee ? 1 : 0;

                string sqlQry = $"UPDATE tbl_Tickets SET CustomerID = {customerID}, ApplyServiceFee = {asf}, customerAKA = '{customerName}' WHERE ID = " + ticketNumber;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateTicketTotalPrice(int ticketID, int ReduceAmount)
        {
            try
            {
                bool result = false;

                string sqlQry = "UPDATE tbl_Tickets SET TotalPrice = TotalPrice - " + ReduceAmount + " WHERE ID = " + ticketID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static void UpdateTicketStatus(int ID, int status, int totalPrice, int serviceFeed, int cash, int creditCard, int transfer, int voucher, int whoClosed, string customerAKA)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Tickets SET CloseAt = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', Shift = " + Settings.Default.Shift + ", Status = " + status + ", PayMethod = 1, TotalPrice = " + totalPrice +
                                ", ServiceFee = " + serviceFeed + ", Cash = " + cash + ", CreditCard = " + creditCard + ", Transfer = " + transfer + ", Voucher  = " + voucher +
                                ", WhoClosed = " + whoClosed + ", customerAKA = '" + customerAKA + "' WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void UpdateTicketStatus(int ID, int cash, int creditCard, int transfer)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET Cash = {cash}, CreditCard = {creditCard}, Transfer = {transfer} WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void UpdateTicketStatusForSplitTicket(int ID, int totalPrice, int serviceFeed, string customerAKA)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET Shift = 0, Status = 1, PayMethod = 0, TotalPrice = {totalPrice}, ServiceFee = {serviceFeed}, Cash = 0, CreditCard = 0, Transfer = 0, customerAKA = '{customerAKA}' WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static bool UpdateTicket (clsTicket tck)
        {
            try
            {
                int status = tck.Status == true ? 1 : 0;
                int splited = tck.Splited == true ? 1 : 0;
                int applyServiceFee = tck.ApplyServiceFee == true ? 1 : 0;

                string sqlQry = "UPDATE tbl_Tickets SET " +
                                "TicketDate = '" + tck.TicketDate + "', " +
                                "CustomerID = " + tck.CustID + ", " +
                                "TotalPrice = " + tck.TotalPrice + ", " +
                                "Payments = " + tck.Payments + ", " +
                                "ServiceFee = " + tck.ServiceFee + ", " +
                                "Cash = " + tck.Cash + ", " +
                                "CreditCard = " + tck.CreditCard + ", " +
                                "Transfer = " + tck.Transfer + ", " +
                                "PayMethod = " + tck.PayMethod + ", " +
                                "Status = " + status + ", " +
                                "ApplyServiceFee = " + applyServiceFee + ", " +
                                "Splited = " + splited + " WHERE ID = " + tck.ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static void UpdateOldTicketStatus(int ID, string newTicketDate, int totalPrice, int cash, int creditCard, int transfer, int whoClosed)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET CloseAt = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', Shift = {Settings.Default.Shift}, Status = 0, TicketDate = '{newTicketDate}', PayMethod = 1, TotalPrice = {totalPrice}, Cash = {cash}, CreditCard = {creditCard}, Transfer = {transfer}, WhoClosed = {whoClosed} WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static bool UpdateOldTicketPayment(int ID, int paymentAmount, int whoClosed)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Tickets SET TotalPrice = TotalPrice - " + paymentAmount + ", Payments = Payments + " + paymentAmount + ", WhoClosed = " + whoClosed + " WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static void UpdateTicketSplited(int ID)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Tickets SET Splited = 1 WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void UpdateTicketDetailGUID(string fromGUID, string toGUID, int ID)
        {
            try
            {
                string sqlQry = string.Empty;
                
                if (ID == 0)
                    sqlQry = $"UPDATE tbl_TicketsDetail SET GUID = '{toGUID}' WHERE GUID = '{fromGUID}'";
                else
                    sqlQry = "UPDATE tbl_TicketsDetail SET GUID = '" + toGUID + "' WHERE GUID = '" + fromGUID + "' AND ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void UpdateTicketDetailSplited(int ID)
        {
            try
            {
                string sqlQry = "UPDATE tbl_TicketsDetail SET Splited = 1 WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void UpdateTicketDetailRemoved(int ID)
        {
            try
            {
                string sqlQry = "DELETE tbl_TicketsDetail WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static bool UpdateCustomerProfile (int ID, int status, int applyService, int freeOfCharge, int creditLimit)
        {
            try
            {
                bool result = false;

                string sqlQry = $"UPDATE tbl_CustomerID SET Active = {status}, ApplyServiceFee = {applyService}, FreeOfCharge = {freeOfCharge}, CreditLimit = {creditLimit} WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateCustomerStatus(int ID, int status)
        {
            try
            {
                bool result = false;
                string dt = DateTime.Now.ToString("yyyyMMdd");

                string sqlQry = "UPDATE tbl_CustomerID SET Active = " + status + ", LastPayment = '" + dt + "' WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateCustomerBirthDate(int ID, string birthDate, int creditLimit)
        {
            try
            {
                bool result = false;

                string sqlQry = "UPDATE tbl_CustomerID SET CreditLimit = " + creditLimit + ", BirthDay = '" + birthDate + "' WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateCustomerMailAddress(int ID, string mailAddress)
        {
            try
            {
                bool result = false;

                string sqlQry = $"UPDATE tbl_CustomerID SET MailAddress = '{mailAddress}' WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateCustomerLoyaltyPoints(int ID, int points)
        {
            try
            {
                bool result = false;

                string sqlQry = $"UPDATE tbl_CustomerID SET LoyaltyPoints = LoyaltyPoints + {points} WHERE ID = " + ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateFeeServiceToOpenTickets()
        {
            try
            {
                List<clsCustomerVIP> lstOpenTickets = new List<clsCustomerVIP>();

                if (Settings.Default.UseNickNames)
                {
                    lstOpenTickets = DB.ListBinding_tbl_OpenTickets();
                }
                else
                {
                    lstOpenTickets = DB.ListBinding_tbl_CustomerID(3, 1);
                }

                foreach (clsCustomerVIP cust in lstOpenTickets)
                {
                    clsTicket ticket = DB.GetTicket(DB.GetTicketNumber(Settings.Default.BusinessDate, cust.ID));

                    if (ticket.ApplyServiceFee)
                    {
                        ticket.ServiceFee = (ticket.TotalPrice * 10) / 100;
                        ticket.TotalPrice += ticket.ServiceFee;
                        DB.UpdateTicket(ticket);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "UpdateFeeServiceToOpenTickets PASSED", Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateFeeServiceToTicket(int ticketNum, bool _action, int serviceFee, int IVAFee)
        {
            try
            {
                int action = _action ? 1 : 0;

                string sqlQry = $"UPDATE tbl_Tickets SET ApplyServiceFee = {action}, ServiceFee = {serviceFee}, IVAFee = {IVAFee} WHERE ID = " + ticketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateCustIDWithDeletedID(int custID)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET CustomerID = {Settings.Default.DeletedID} WHERE CustomerID = " + custID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateTemporalTablesIDWithDeletedID()
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET CustomerID = {Settings.Default.DeletedID} WHERE CustomerID IN (SELECT ID FROM tbl_CustomerID WHERE Type = 2 AND SubType = 1); " +
                                 "DELETE tbl_CustomerID WHERE Type = 2 AND SubType = 1";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool CancelTicket(int ticketNum, int whoCancel, int PayMethod)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Tickets SET CloseAt = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', Shift = {Settings.Default.Shift}, TotalPrice = 0, ServiceFee = 0, Payments = 0, Cash = 0, CreditCard = 0, Transfer = 0, PayMethod = {PayMethod}, Status = 0, WhoClosed = {whoCancel} WHERE ID = {ticketNum}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool ReassignCustomerID(int ticketNum, int newID)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Tickets SET CustomerID = " + newID + " WHERE ID = " + ticketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                sqlQry = "UPDATE tbl_DailyClosing SET CustomerID = " + newID + " WHERE TicketNumber = " + ticketNum;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool ReassignOpenTicket(int oldID, string newID)
        {
            try
            {
                clsCustomerVIP newCust = DB.GetCustomerProfile(newID);

                int applyServiceFee = newCust.ApplyServiceFee ? 1 : 0;

                string sqlQry = "UPDATE tbl_OpenTickets SET ID = " + newCust.ID + ", Type = " + newCust.Type + ", CustomerID = '" + newCust.CustomerID + "', ApplyServiceFee = " + applyServiceFee + " WHERE ID = " + oldID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool RenameCustomerAKA(int ticketNum, int ID, string newCustAKA)
        {
            try
            {
                bool result = false;

                // update two tables in one query
                string sqlQry = $"UPDATE tbl_OpenTickets SET CustomerID = '{newCustAKA}' WHERE ID = {ID}; UPDATE tbl_Tickets SET customerAKA = '{newCustAKA}' WHERE ID = {ticketNum}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool ChangeItemPriceInTicket(clsTicketDetail item, int newPrice)
        {
            try
            {
                bool result = false;

                // update two tables in one query
                string sqlQry = $"UPDATE tbl_TicketsDetail SET UnitPrice = {newPrice}, TotalPrice = {newPrice} WHERE ID = '{item.ID}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool AssignShiftToDailyClosing(int currentShift, string workDay)
        {
            try
            {
                bool result = false;

                // update two tables in one query
                string sqlQry = $"UPDATE tbl_Tickets SET Splited = {currentShift} WHERE Status = 0 AND Splited = 0 AND TicketDate = '{workDay}'; " +
                                $"UPDATE tbl_Payments SET Splited = {currentShift} WHERE Splited = 0 AND PaymentDate = '{workDay}'; " +
                                $"UPDATE tbl_Expenses SET Splited = {currentShift} WHERE Splited = 0 AND ExpenseDate = '{workDay}'; " +
                                $"UPDATE tbl_TicketsOldCancelled SET Splited = {currentShift} WHERE Splited = 0 AND PayDate = '{workDay}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool SetATVStatus(int ticketNumber, ATVResponse atvDeserialized, int action)
        {
            try
            {
                bool result = false;
                string sqlQry = string.Empty;

                switch (action)
                {
                    case 1:
                        sqlQry = $"UPDATE tbl_Tickets SET ATVStatusCode = {atvDeserialized.cod}, ATVInternalID = {atvDeserialized.internal_id}, ATVConsecutive = '{atvDeserialized.consecutivo}', ATVKey = '{atvDeserialized.clave}', ATVStateMsj = '{atvDeserialized.estado}', ATVErrorMsj = '{atvDeserialized.msj}' WHERE ID = {ticketNumber}";
                        break;
                    case 2:
                        sqlQry = $"UPDATE tbl_Tickets SET ATVStateMsj = '{atvDeserialized.estado}' WHERE ID = {ticketNumber}";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                if (atvDeserialized.cod < 200)
                {
                    string msg = $"SetATVStatus ERROR: Code {atvDeserialized.cod}: {atvDeserialized.msj}";
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, msg, Logger.Severity.ERROR);
                    Helper.ShowMessage(msg, System.Windows.Forms.MessageBoxIcon.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static bool UpdateCashOnHandAtTheBeginning(int initialCash)
        {
            try
            {
                bool result = false;

                string sqlQry = $"UPDATE tbl_CustomerID SET CreditLimit = {initialCash} WHERE CustomerID = 'AWCDIGITALCOMMERCE'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region EXPENSES MGMT
        public static bool InsertNewExpense(string dt, string expDesc, double expAmt)
        {
            try
            {
                bool result = false;

                string sqlQry = "INSERT INTO tbl_Expenses (ExpenseDate, ExpenseDescription, ExpenseAmount) " +
                                "VALUES ('" + dt + "', '" + expDesc + "', " + expAmt + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        public static List<clsExpense> GetExpenses()
        {
            try
            {
                List<clsExpense> expensesList = new List<clsExpense>();

                string sqlQry = "SELECT ID, ExpenseDate, ExpenseDescription, ExpenseAmount FROM tbl_Expenses";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsExpense expense = new clsExpense();

                            expense.ID = Convert.ToInt32(sdr["ID"]);
                            expense.ExpenseDate = ConverTicketDate(sdr["ExpenseDate"].ToString());
                            expense.ExpenseDescription = sdr["ExpenseDescription"].ToString();
                            expense.ExpenseAmount = Convert.ToInt32(sdr["ExpenseAmount"]);
                            expensesList.Add(expense);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return expensesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsExpense> GetExpenses(string dt)
        {
            try
            {
                List<clsExpense> expensesList = new List<clsExpense>();

                string sqlQry = $"SELECT * FROM tbl_Expenses WHERE Shift = {Settings.Default.ShiftForQuery} AND ExpenseDate = '{dt}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsExpense expense = new clsExpense();

                            expense.ID = Convert.ToInt32(sdr["ID"]);
                            expense.ExpenseDate = sdr["ExpenseDate"].ToString();
                            expense.ExpenseDescription = sdr["ExpenseDescription"].ToString();
                            expense.ExpenseAmount = Convert.ToDouble(sdr["ExpenseAmount"]);

                            expensesList.Add(expense);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return expensesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsLunch> GetLunches(string dt)
        {
            try
            {
                List<clsLunch> lunchesList = new List<clsLunch>();

                string sqlQry = "SELECT * FROM tbl_Lunches WHERE LunchDate = '" + dt + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsLunch lunch = new clsLunch();

                            lunch.ID = Convert.ToInt32(sdr["ID"]);
                            lunch.LunchDate = sdr["LunchDate"].ToString();
                            lunch.GUID = sdr["GUID"].ToString();
                            lunch.EmployeeName = sdr["EmployeeName"].ToString();
                            lunch.Qty = Convert.ToInt32(sdr["Qty"]);
                            lunch.MealID = Convert.ToInt32(sdr["MealID"]);

                            lunchesList.Add(lunch);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return lunchesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsSmallPayment> GetSmallPayments(string dt)
        {
            try
            {
                List<clsSmallPayment> smlPayList = new List<clsSmallPayment>();

                string sqlQry = string.Empty;
                
                if (dt.Length > 0)
                    sqlQry = $"SELECT * FROM tbl_Payments WHERE Shift = {Settings.Default.ShiftForQuery} AND PaymentDate = '{dt}' ORDER BY TicketID";
                else
                    sqlQry = "SELECT * FROM tbl_Payments ORDER BY PaymentDate, TicketID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsSmallPayment smlPay = new clsSmallPayment();

                            smlPay.ID = Convert.ToInt32(sdr["ID"]);
                            smlPay.PaymentDate = ConverTicketDate(sdr["PaymentDate"].ToString());
                            smlPay.RandomRef = sdr["RandomRef"].ToString();
                            smlPay.CustomerID = Convert.ToInt32(sdr["CustomerID"]);
                            smlPay.TicketID = Convert.ToInt32(sdr["TicketID"]);
                            smlPay.CurTotalPrice = Convert.ToInt32(sdr["CurTotalPrice"]);
                            smlPay.PaymentAmount = Convert.ToInt32(sdr["PaymentAmount"]);
                            smlPay.Cash = Convert.ToInt32(sdr["Cash"]);
                            smlPay.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                            smlPay.Transfer = Convert.ToInt32(sdr["Transfer"]);
                            smlPay.NewTotalPrice = Convert.ToInt32(sdr["NewTotalPrice"]);
                            smlPay.WhoClosed = Convert.ToInt32(sdr["WhoClosed"]);

                            smlPayList.Add(smlPay);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return smlPayList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsSmallPayment> GetSmallPayments(string dt1, string dt2)
        {
            try
            {
                List<clsSmallPayment> smlPayList = new List<clsSmallPayment>();

                string sqlQry = "SELECT * FROM tbl_Payments WHERE PaymentDate >= '" + dt1 + "' AND PaymentDate <= '" + dt2 + "' ORDER BY TicketID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsSmallPayment smlPay = new clsSmallPayment();

                            smlPay.ID = Convert.ToInt32(sdr["ID"]);
                            smlPay.PaymentDate = ConverTicketDate(sdr["PaymentDate"].ToString());
                            smlPay.RandomRef = sdr["RandomRef"].ToString();
                            smlPay.CustomerID = Convert.ToInt32(sdr["CustomerID"]);
                            smlPay.TicketID = Convert.ToInt32(sdr["TicketID"]);
                            smlPay.CurTotalPrice = Convert.ToInt32(sdr["CurTotalPrice"]);
                            smlPay.PaymentAmount = Convert.ToInt32(sdr["PaymentAmount"]);
                            smlPay.Cash = Convert.ToInt32(sdr["Cash"]);
                            smlPay.CreditCard = Convert.ToInt32(sdr["CreditCard"]);
                            smlPay.Transfer = Convert.ToInt32(sdr["Transfer"]);
                            smlPay.NewTotalPrice = Convert.ToInt32(sdr["NewTotalPrice"]);
                            smlPay.WhoClosed = Convert.ToInt32(sdr["WhoClosed"]);

                            smlPayList.Add(smlPay);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return smlPayList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsSmallPayment GetSmallPaymentsSummary(string dt)
        {
            try
            {
                clsSmallPayment smlPay = new clsSmallPayment(); ;

                string sqlQry = $"SELECT SUM(Cash) AS 'Cash', SUM(CreditCard) AS 'CreditCard', SUM(Transfer) AS 'Transfer' FROM tbl_Payments WHERE Shift = {Settings.Default.ShiftForQuery} AND PaymentDate = '{dt}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        smlPay.Cash = DBNull.Value.Equals(sdr["Cash"]) ? 0 : Convert.ToInt32(sdr["Cash"]);
                        smlPay.CreditCard = DBNull.Value.Equals(sdr["CreditCard"]) ? 0 : Convert.ToInt32(sdr["CreditCard"]);
                        smlPay.Transfer = DBNull.Value.Equals(sdr["Transfer"]) ? 0 : Convert.ToInt32(sdr["Transfer"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return smlPay;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsSmallPayment GetSmallPaymentsSummary(string dt1, string dt2)
        {
            try
            {
                clsSmallPayment smlPay = new clsSmallPayment(); ;

                string sqlQry = "SELECT SUM(Cash) AS 'Cash', SUM(CreditCard) AS 'CreditCard', SUM(Transfer) AS 'Transfer' FROM tbl_Payments WHERE PaymentDate >= '" + dt1 + "' AND PaymentDate <= '" + dt2 + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        smlPay.Cash = DBNull.Value.Equals(sdr["Cash"]) ? 0 : Convert.ToInt32(sdr["Cash"]);
                        smlPay.CreditCard = DBNull.Value.Equals(sdr["CreditCard"]) ? 0 : Convert.ToInt32(sdr["CreditCard"]);
                        smlPay.Transfer = DBNull.Value.Equals(sdr["Transfer"]) ? 0 : Convert.ToInt32(sdr["Transfer"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return smlPay;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region PROVIDERS MGMT
        public static List<clsProvider> GetProvidersCatalog()
        {
            try
            {
                List<clsProvider> providersList = new List<clsProvider>();

                string sqlQry = "SELECT * FROM tbl_Providers";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsProvider provider = new clsProvider();

                            provider.ID = Convert.ToInt32(sdr["ID"]);
                            provider.ProviderName = sdr["ProviderName"].ToString();
                            provider.BusinessAddress = sdr["BusinessAddress"].ToString();
                            provider.eMailAddress = sdr["eMailAddress"].ToString();
                            provider.PaymentMethod = sdr["PaymentMethod"].ToString();
                            provider.PhoneNumber = sdr["PhoneNumber"].ToString();
                            provider.CellularNumber = sdr["CellularNumber"].ToString();
                            provider.Remarks = sdr["Remarks"].ToString();
                            providersList.Add(provider);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return providersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsProvider CheckProviderName(string providerName)
        {
            try
            {
                clsProvider providerProfile = new clsProvider();

                string sqlQry = "SELECT * FROM tbl_Providers WHERE ProviderName = '" + providerName + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        providerProfile.ID = Convert.ToInt32(sdr["ID"]);
                        providerProfile.BusinessAddress = sdr["BusinessAddress"].ToString();
                        providerProfile.eMailAddress = sdr["eMailAddress"].ToString();
                        providerProfile.PaymentMethod = sdr["PaymentMethod"].ToString();
                        providerProfile.PhoneNumber = sdr["PhoneNumber"].ToString();
                        providerProfile.CellularNumber = sdr["CellularNumber"].ToString();
                        providerProfile.Remarks = sdr["Remarks"].ToString();
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return providerProfile;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static bool InsertNewProvider(clsProvider providerProfile)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Providers (ProviderName, BusinessAddress, eMailAddress, PaymentMethod, PhoneNumber, CellularNumber, Remarks) " +
                                "VALUES ('" + providerProfile.ProviderName + "', '" +
                                              providerProfile.BusinessAddress + "', '" +
                                              providerProfile.eMailAddress + "', '" +
                                              providerProfile.PaymentMethod + "', '" +
                                              providerProfile.PhoneNumber + "', '" +
                                              providerProfile.CellularNumber + "', '" +
                                              providerProfile.Remarks + "')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteProvider(int providerID)
        {
            try
            {
                string sqlQry = "DELETE FROM tbl_Providers WHERE ID = " + providerID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateProvider(clsProvider p)
        {
            try
            {
                string sqlQry = "UPDATE tbl_Providers SET " +
                                "ProviderName = '" + p.ProviderName + "', " +
                                "BusinessAddress = '" + p.BusinessAddress + "', " +
                                "eMailAddress = '" + p.eMailAddress + "', " +
                                "PaymentMethod = '" + p.PaymentMethod + "', " +
                                "PhoneNumber = '" + p.PhoneNumber + "', " +
                                "CellularNumber = '" + p.CellularNumber + "', " +
                                "Remarks = '" + p.Remarks + "' WHERE ID = " + p.ID;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region INVOICES MGMT
        public static int  CheckProviderAndInvoice(int providerID, int invoiceNumber)
        {
            try
            {
                int invoiceID = 0;

                string sqlQry = "SELECT InvoiceID FROM tbl_Invoices WHERE ProviderID = " + providerID + " AND InvoiceNumber = " + invoiceNumber;

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        invoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return invoiceID;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }
        #endregion

        #region INVENTORIES MANAGEMENT
        // updates
        public static bool InsertNewInvoice(clsInvoice newInvoice)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Invoices (InvoiceNumber, InvoiceDate, ProviderID, InvoiceAmount, InvoiceGUID) " +
                                "VALUES (" + newInvoice.InvoiceNumber + ", '"
                                           + newInvoice.InvoiceDate + "', "
                                           + newInvoice.ProviderID + ", "
                                           + newInvoice.InvoiceAmount + ", '"
                                           + newInvoice.InvoiceGUID + "')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertNewInvoiceItem(clsInvoiceItem newInvoiceItem)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_InvoicesDetail (InvoiceGUID, ItemType, ItemID, ItemQty)" +
                                "VALUES ('" + newInvoiceItem.InvoiceGUID + "', "
                                            + newInvoiceItem.ItemType + ", "
                                            + newInvoiceItem.ItemID + ", "
                                            + newInvoiceItem.ItemQty + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertNewNote(clsNote newNote)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_Notes (NoteDate, NoteType, NoteDescription, NoteAmount, NoteGUID) " +
                                "VALUES ('" + newNote.NoteDate + "', " +
                                              newNote.NoteType + ", '" +
                                              newNote.NoteDescription + "', " +
                                              newNote.NoteAmount + ", '" +
                                              newNote.NoteGUID + "')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertNewNoteDetail(clsNoteDetail newNoteDetail)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_NotesDetail (NoteGUID, ItemType, ItemID, ItemQty, ItemPrice, ItemTotal)" +
                                "VALUES ('" + newNoteDetail.NoteGUID + "', "
                                            + newNoteDetail.ItemType + ", "
                                            + newNoteDetail.ItemID + ", "
                                            + newNoteDetail.ItemQty + ", "
                                            + newNoteDetail.ItemPrice + ", "
                                            + newNoteDetail.ItemTotal + ")";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertNewDefectiveItem(clsItemDefective ItemDefective)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_ItemsDefective (ItemID, ItemQty, DeclarationDate, whoDeclared)" +
                                "VALUES (" + ItemDefective.ItemID + ", "
                                           + ItemDefective.ItemQty + ", '"
                                           + ItemDefective.DeclarationDate + "', "
                                           + ItemDefective.whoDeclared + ")";
                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateItemInventory(string action, clsItem Item)
        {
            try
            {
                string sqlQry = string.Empty;

                switch (action)
                {
                    case "ADD":
                        sqlQry = "UPDATE tbl_Items SET ItemAvailable = ItemAvailable + " + Item.ItemSold + " WHERE ID = " + Item.ID;
                        break;
                    case "SAL":
                        {
                            clsItem it = GetItem(Item.ID);

                            switch (it.ItemSubType)
                            {
                                // item standard
                                case 0:
                                case 3:
                                case 4:
                                    sqlQry = "UPDATE tbl_Items SET ItemAvailable = ItemAvailable - " + Item.ItemSold + ", ItemSold = ItemSold + " + Item.ItemSold + " WHERE ID = " + Item.ID;
                                    break;
                                // item with parent
                                case 1:
                                case 2:
                                    sqlQry = "UPDATE tbl_Items SET ItemSold = ItemSold + " + Item.ItemSold + " WHERE ID = " + Item.ID + "; " +
                                             "UPDATE tbl_Items SET ItemAvailable = ItemAvailable - " + (Item.ItemSold * it.ItemParentUnit) + ", ItemSold = ItemSold + " + (Item.ItemSold * it.ItemParentUnit) + " WHERE ID = " + it.ItemParent;
                                    break;
                            }
                            break;
                        }
                    case "DEF":
                        sqlQry = "UPDATE tbl_Items SET ItemAvailable = ItemAvailable - " + Item.ItemDefective + ", ItemDefective = ItemDefective + " + Item.ItemDefective + " WHERE ID = " + Item.ID;
                        break;
                    case "INI":
                        sqlQry = "UPDATE tbl_Items SET ItemAvailable = " + Item.ItemAvailable +
                                 ", ItemSold = " + Item.ItemSold  +
                                 ", ItemDefective = " + Item.ItemDefective +
                                 ", ItemSubtype = " + Item.ItemSubType +
                                 ", ItemParent = " + Item.ItemParent +
                                 ", ItemParentUnit = " + Item.ItemParentUnit +
                                 ", ItemMinimum = " + Item.ItemMinimum +
                                 ", ItemStock = " + Item.ItemStock +
                                 " WHERE ID = " + Item.ID;
                        break;
                    case "NOR":
                        if (Item.ItemType == 0)
                        {
                            sqlQry = "UPDATE tbl_Items SET ItemAvailable = 0, ItemSold = 0, ItemDefective = 0, ItemMinimum = 0, DebitNotes = 0, CreditNotes = 0";
                        }
                        else
                        {
                            sqlQry = $"UPDATE tbl_Items SET ItemAvailable = 0, ItemSold = 0, ItemDefective = 0, ItemMinimum = 0, DebitNotes = 0, CreditNotes = 0 WHERE ItemType = {Item.ItemType}";
                        }
                        break;
                    case "DEB":
                        sqlQry = "UPDATE tbl_Items SET ItemAvailable = ItemAvailable - " + Item.ItemSold + ", DebitNotes =  DebitNotes + " + Item.ItemSold + "WHERE ID = " + Item.ID;
                        break;
                    case "CRED":
                        sqlQry = "UPDATE tbl_Items SET ItemAvailable = ItemAvailable + " + Item.ItemSold + ", CreditNotes =  CreditNotes + " + Item.ItemSold + "WHERE ID = " + Item.ID;
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static void ApplyOpenTicketsToInventory(string workDay)
        {
            try
            {
                string sqlQry = "SELECT ItemID, SUM(Qty) AS 'Qty' FROM tbl_TicketsDetail " +
                                "WHERE GUID IN (SELECT GUID FROM [dbo].[tbl_Tickets] " +
                                "WHERE (Status = 1) AND (TicketDate = '" + workDay + "') AND (ID NOT IN (SELECT TicketNumber FROM tbl_DailyClosing))) " +
                                "GROUP BY ItemID";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();
                            item.ID = sdr.GetInt32(0);
                            item.ItemSold = sdr.GetInt32(1);
                            DB.UpdateItemInventory("SAL", item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        public static void ApplyZeroToItemSoldAtInventory()
        {
            try
            {
                string sqlQry = "  UPDATE tbl_Items SET ItemSold = 0";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        // queries
        public static List<clsInvoice> GetInvoicesListByYearMonth(string yearMonth)
        {
            try
            {
                List<clsInvoice> invoicesList = new List<clsInvoice>();

                string sqlQuery = "SELECT InvoiceNumber, InvoiceDate, tbl_Providers.ProviderName AS ProviderName, InvoiceAmount, InvoiceGUID FROM tbl_Invoices " +
                                  "INNER JOIN tbl_Providers ON tbl_Providers.ID = tbl_Invoices.ProviderID WHERE InvoiceDate LIKE '" + yearMonth + "' ORDER BY InvoiceDate";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsInvoice invoice = new clsInvoice();

                            string invoiceDate = sdr["InvoiceDate"].ToString();
                            invoiceDate = invoiceDate.Substring(6, 2) + "." + invoiceDate.Substring(4, 2) + "." + invoiceDate.Substring(0, 4);
                            invoice.InvoiceDate = invoiceDate;
                            invoice.InvoiceNumber = Convert.ToInt32(sdr["InvoiceNumber"]);
                            invoice.ProviderName = sdr["ProviderName"].ToString();
                            invoice.InvoiceAmount = Convert.ToDouble(sdr["InvoiceAmount"]);
                            invoice.InvoiceGUID = sdr["InvoiceGUID"].ToString();

                            invoicesList.Add(invoice);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return invoicesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsInvoiceItem> GetInvoiceItemsByGUID(string invoiceGUID)
        {
            try
            {
                List<clsInvoiceItem> invoiceItemsList = new List<clsInvoiceItem>();

                string sqlQuery = "SELECT tbl_InvoicesDetail.ItemID AS ItemID, tbl_Items.ItemDescription AS ItemDescription, tbl_InvoicesDetail.ItemQty AS ItemQty  FROM tbl_InvoicesDetail " +
                                  "INNER JOIN tbl_Items ON tbl_Items.ID = tbl_InvoicesDetail.ItemID WHERE InvoiceGUID = '" + invoiceGUID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsInvoiceItem invoiceItem = new clsInvoiceItem();

                            invoiceItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            invoiceItem.ItemDescription= sdr["ItemDescription"].ToString();
                            invoiceItem.ItemQty = Convert.ToInt32(sdr["ItemQty"]);

                            invoiceItemsList.Add(invoiceItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return invoiceItemsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItemDefective> GetDefectivesItemsByYearMonth(string yearMonth)
        {
            try
            {
                List<clsItemDefective> defectiveItemList = new List<clsItemDefective>();

                string sqlQuery = "SELECT DeclarationDate, ItemID, tbl_Items.ItemDescription AS ItemDescription, ItemQty, tbl_Users.userName AS UserName FROM tbl_ItemsDefective " +
                                  "INNER JOIN tbl_Items ON tbl_Items.ID = tbl_ItemsDefective.ItemID " +
                                  "INNER JOIN tbl_Users ON tbl_Users.userPIN = tbl_ItemsDefective.whoDeclared";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemDefective defItem = new clsItemDefective();

                            string defDate = sdr["DeclarationDate"].ToString();
                            defDate = defDate.Substring(6, 2) + "." + defDate.Substring(4, 2) + "." + defDate.Substring(0, 4);
                            defItem.DeclarationDate = defDate;
                            defItem.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            defItem.ItemDescription = sdr["ItemDescription"].ToString();
                            defItem.ItemQty = Convert.ToInt32(sdr["ItemQty"]);
                            defItem.whoDeclaredName = sdr["UserName"].ToString();

                            defectiveItemList.Add(defItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return defectiveItemList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> GetItemsBelowMinimum()
        {
            try
            {
                List<clsItem> minimumItemList = new List<clsItem>();

                string sqlQuery = "SELECT * FROM tbl_Items WHERE ItemMinimum > 0 AND ItemAvailable <= ItemMinimum";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem minimumItem = new clsItem();

                            minimumItem.ID = Convert.ToInt32(sdr["ID"]);
                            minimumItem.ItemDescription = sdr["ItemDescription"].ToString();
                            minimumItem.ItemMinimum = Convert.ToInt32(sdr["ItemMinimum"]);
                            minimumItem.ItemAvailable = Convert.ToInt32(sdr["ItemAvailable"]);
                            minimumItem.ItemStock = Convert.ToInt32(sdr["ItemStock"]);

                            minimumItemList.Add(minimumItem);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return minimumItemList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsItem> GetItemsBelowZero()
        {
            try
            {
                List<clsItem> itemBelowZeroList = new List<clsItem>();

                string sqlQuery = "SELECT ID, ItemDescription, ItemAvailable, ItemMinimum FROM tbl_Items WHERE ItemType < 3 AND ItemAvailable <= 0";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem itemBelowZero = new clsItem();

                            itemBelowZero.ID = Convert.ToInt32(sdr["ID"]);
                            itemBelowZero.ItemDescription = sdr["ItemDescription"].ToString();
                            itemBelowZero.ItemMinimum = Convert.ToInt32(sdr["ItemMinimum"]);
                            itemBelowZero.ItemAvailable = Convert.ToInt32(sdr["ItemAvailable"]);

                            itemBelowZeroList.Add(itemBelowZero);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return itemBelowZeroList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsNote> GetNotesListByYearMonth(string yearMonth)
        {
            try
            {
                List<clsNote> notesList = new List<clsNote>();

                string sqlQuery = "SELECT * FROM tbl_Notes WHERE NoteDate LIKE '" + yearMonth + "' ORDER BY NoteDate";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsNote note = new clsNote();

                            note.ID = Convert.ToInt32(sdr["ID"]);

                            string noteDate = sdr["NoteDate"].ToString();
                            noteDate = noteDate.Substring(6, 2) + "." + noteDate.Substring(4, 2) + "." + noteDate.Substring(0, 4);
                            note.NoteDate = noteDate;

                            note.NoteType = Convert.ToInt32(sdr["NoteType"]);
                            note.NoteDescription = sdr["NoteDescription"].ToString();
                            note.NoteAmount = Convert.ToInt32(sdr["NoteAmount"]);
                            note.NoteGUID = sdr["NoteGUID"].ToString();

                            notesList.Add(note);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return notesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsNoteDetail> GetNoteDetailByGUID(string noteGUID)
        {
            try
            {
                List<clsNoteDetail> noteDetailList = new List<clsNoteDetail>();

                string sqlQuery = "SELECT tbl_NotesDetail.ItemID AS ItemID, tbl_Items.ItemDescription AS ItemDescription, tbl_NotesDetail.ItemQty AS ItemQty  FROM tbl_NotesDetail " +
                                  "INNER JOIN tbl_Items ON tbl_Items.ID = tbl_NotesDetail.ItemID WHERE NoteGUID = '" + noteGUID + "'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsNoteDetail noteDetail = new clsNoteDetail();

                            noteDetail.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            noteDetail.ItemDescription = sdr["ItemDescription"].ToString();
                            noteDetail.ItemQty = Convert.ToInt32(sdr["ItemQty"]);

                            noteDetailList.Add(noteDetail);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQuery, Logger.Severity.DEBUG);

                return noteDetailList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region LOYALTY REWARDS
        public static List<clsLoyaltyReward> ListBinding_tbl_LoyaltyRewards()
        {
            try
            {
                List<clsLoyaltyReward> lstItems = new List<clsLoyaltyReward>();

                string sqlQuery = "SELECT * FROM tbl_LoyaltyRewards ORDER BY Description ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsLoyaltyReward item = new clsLoyaltyReward();
                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.Description = sdr["Description"].ToString();
                            item.Status = sdr["Status"].ToString();
                            item.ItemToQualify = Convert.ToInt32(sdr["ItemToQualify"]);
                            item.QtyToQualify = Convert.ToInt32(sdr["QtyToQualify"]);
                            item.ItemRewarded = Convert.ToInt32(sdr["ItemRewarded"]);
                            item.QtyRewarded = Convert.ToInt32(sdr["QtyRewarded"]);
                            item.TotalItemsAwarded = Convert.ToInt32(sdr["TotalItemsAwarded"]);
                            lstItems.Add(item);
                        }
                    }
                }
                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsLoyaltyReward GetLoyaltyReward(string desc)
        {
            try
            {
                clsLoyaltyReward item = new clsLoyaltyReward();

                string sqlQry = $"SELECT * FROM tbl_LoyaltyRewards WHERE Description = '{desc.ToUpper()}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        item.ID = Convert.ToInt32(sdr["ID"]);
                        item.Description = sdr["Description"].ToString();
                        item.Status = sdr["Status"].ToString();
                        item.ItemToQualify = Convert.ToInt32(sdr["ItemToQualify"]);
                        item.QtyToQualify = Convert.ToInt32(sdr["QtyToQualify"]);
                        item.ItemRewarded = Convert.ToInt32(sdr["ItemRewarded"]);
                        item.QtyRewarded = Convert.ToInt32(sdr["QtyRewarded"]);
                        item.TotalItemsAwarded = Convert.ToInt32(sdr["TotalItemsAwarded"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return item;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static bool InsertLoyaltyReward(clsLoyaltyReward loyRew)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_LoyaltyRewards (Description, Status, ItemToQualify, QtyToQualify, ItemRewarded, QtyRewarded) " +
                               $"VALUES ('{loyRew.Description}', '{loyRew.Status}', {loyRew.ItemToQualify}, {loyRew.QtyToQualify}, {loyRew.ItemRewarded}, {loyRew.QtyRewarded})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool UpdateLoyaltyReward(clsLoyaltyReward loyRew)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_LoyaltyRewards SET Description = '{loyRew.Description}', Status = '{loyRew.Status}', ItemToQualify = {loyRew.ItemToQualify}, QtyToQualify = {loyRew.QtyToQualify}, ItemRewarded = {loyRew.ItemRewarded}, QtyRewarded = {loyRew.QtyRewarded}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool DeleteLoyaltyReward(int ID)
        {
            try
            {
                bool result = false;

                string sqlQry = $"DELETE FROM tbl_LoyaltyRewards WHERE ID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    result = true;
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return result;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false; ;
            }
        }
        #endregion

        #region PREFIXES
        public static bool InsertPrefix(int type, string prefix)
        {
            try
            {
                if (prefix.Length == 0)
                    return true;

                string sqlQry = $"INSERT INTO tbl_Prefixes (Type, Prefix) VALUES ({type}, '{prefix.ToUpper()}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"PREFIJO {prefix.ToUpper()} YA EXISTE EN LA BASE DE DATOS", Logger.Severity.INFORMATION);
                DB.UpdatePrefixHits(type, prefix);
                return false;
            }
        }
        public static bool UpdatePrefixHits(int type, string prefix)
        {
            try
            {
                string sqlQry = $"UPDATE tbl_Prefixes SET Hits = Hits + 1 WHERE Type = {type} AND Prefix = '{prefix.ToUpper()}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static List<string> GetPrefixesByType(int type)
        {
            try
            {
                if (type == 0)
                {
                    return null;
                }

                List<string> prefixesList = new List<string>();


                string sqlQry = type == 4 ? $"SELECT Prefix FROM tbl_Prefixes" : $"SELECT TOP 10 Prefix FROM tbl_Prefixes WHERE Type = {type}";

                sqlQry += " ORDER BY Hits DESC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            prefixesList.Add(sdr["Prefix"].ToString());
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return prefixesList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region CATEGORIES
        public static List<clsCategory> ListBinding_tbl_Categories()
        {
            try
            {
                List<clsCategory> lstItems = new List<clsCategory>();

                string sqlQry = "SELECT * FROM tbl_Categories ORDER BY Description ASC";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsCategory item = new clsCategory();
                            item.CategoryID = Convert.ToInt32(sdr["CategoryID"]);
                            item.ParentID = Convert.ToInt32(sdr["ParentID"]);
                            item.Description = sdr["Description"].ToString();
                            lstItems.Add(item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return lstItems;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static clsCategory GetCategory(string desc)
        {
            try
            {
                clsCategory item = new clsCategory();

                string sqlQry = $"SELECT * FROM tbl_Categories WHERE Description = '{desc.ToUpper()}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        item.CategoryID = Convert.ToInt32(sdr["CategoryID"]);
                        item.ParentID = Convert.ToInt32(sdr["ParentID"]);
                        item.Description = sdr["Description"].ToString();
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return item;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion

        #region BARTENDERORDER
        public static string InsertBartenderOrder(string custName, string beveragesList)
        {
            try
            {
                Guid guidID = Guid.NewGuid();

                string sqlQry = $"INSERT INTO tbl_BartenderOrder (GUID,CustomerID,BeveragesList) VALUES ('{guidID.ToString()}', '{custName}', '{beveragesList}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return guidID.ToString();
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return string.Empty;
            }
        }

        public static bool DeleteBartenderOrder(string guid)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_BartenderOrder WHERE GUID = '{guid}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"DeleteBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static clsBartenderOrder GetBartenderOrder()
        {
            try
            {
                clsBartenderOrder order = new clsBartenderOrder();

                string sqlQry = $"SELECT TOP 1 * FROM tbl_BartenderOrder";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        order.GUID = sdr["GUID"].ToString();
                        order.CustomerID = sdr["CustomerID"].ToString();
                        order.BeveragesList = sdr["BeveragesList"].ToString();
                    }
                }
                return order;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"GetBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return null;
            }
        }
        public static List<clsBartenderOrder> GetBartenderOrdersList()
        {
            try
            {
                List <clsBartenderOrder> ordersList = new List<clsBartenderOrder>();

                string sqlQry = $"SELECT TOP 2 * FROM tbl_BartenderOrder";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsBartenderOrder order = new clsBartenderOrder();

                            order.GUID = sdr["GUID"].ToString();
                            order.CustomerID = sdr["CustomerID"].ToString();
                            order.BeveragesList = sdr["BeveragesList"].ToString();
                            
                            ordersList.Add(order);
                        }
                    }
                }
                return ordersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"GetBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return null;
            }
        }
        #endregion

        #region PRINTTICKETREMOTELY
        public static string InsertTicketToPrintRemotely(string xmlString)
        {
            try
            {
                Guid guidID = Guid.NewGuid();

                string sqlQry = $"INSERT INTO tbl_PrintTicketRemotely (GUID, TicketForDataGrid) VALUES ('{guidID.ToString()}', '{xmlString}')";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return guidID.ToString();
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return string.Empty;
            }
        }
        public static bool DeleteTicketPrintedRemotely(string guid)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_PrintTicketRemotely WHERE GUID = '{guid}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"DeleteTicketPrintedRemotely ERROR: {ex}", Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static clsPrintTicketRemotely GetTicketToPrintRemotely()
        {
            try
            {
                clsPrintTicketRemotely ticket = new clsPrintTicketRemotely();

                string sqlQry = $"SELECT TOP 1 * FROM tbl_PrintTicketRemotely";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        ticket.GUID = sdr["GUID"].ToString();
                        ticket.TicketForDataGrid = sdr["TicketForDataGrid"].ToString();
                    }
                }

                //if (Settings.Default.DebugTrace)
                //    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ticket;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region ITEMSORDER
        public static bool InsertItemOrder(string PIN, string itemDesc, int qty)
        {
            try
            {
                int itemID = DB.GetIDByItemDescription(itemDesc);

                string sqlQry = $"INSERT INTO tbl_ItemsOrders (TicketDate, WhoOrder, ItemID, Qty) VALUES ('{Settings.Default.BusinessDate}', '{PIN}', {itemID}, {qty})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"InsertItemOrder ERROR: {ex}", Logger.Severity.ERROR);
                return false;
            }
        }
        public static List<clsItemsOrders> GetItemsOrderByDate(string startDate, string finishDate, int option)
        {
            try
            {
                string sqlQry = string.Empty;

                List<clsItemsOrders> ItemsOrdersList = new List<clsItemsOrders>();

                switch (option)
                {
                    case 0:
                        sqlQry = "SELECT TicketDate, tbl_Users.userName AS 'UserName', tbl_Items.ItemDescription AS 'ItemDescription', SUM(Qty) AS 'Qty' from tbl_ItemsOrders " +
                                 "INNER JOIN tbl_Users ON tbl_Users.userPIN = tbl_ItemsOrders.WhoOrder " +
                                $"INNER JOIN tbl_Items on tbl_Items.ID = tbl_ItemsOrders.ItemID WHERE TicketDate >= '{startDate}' AND TicketDate <= '{finishDate}' " +
                                 "GROUP BY TicketDate, ItemDescription, UserName";
                        break;
                    case 1:
                        sqlQry = "SELECT tbl_Users.userName AS 'UserName', tbl_Items.ItemDescription AS 'ItemDescription', SUM(Qty) AS 'Qty' from tbl_ItemsOrders " +
                                 "INNER JOIN tbl_Users ON tbl_Users.userPIN = tbl_ItemsOrders.WhoOrder " +
                                $"INNER JOIN tbl_Items on tbl_Items.ID = tbl_ItemsOrders.ItemID WHERE TicketDate >= '{startDate}' AND TicketDate <= '{finishDate}' " +
                                 "GROUP BY UserName, ItemDescription";
                        break;
                }

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItemsOrders itemOrder = new clsItemsOrders();

                            itemOrder.TicketDate = option == 0 ? ConverTicketDate(sdr["TicketDate"].ToString()) : string.Empty;
                            itemOrder.WhoOrder = sdr["UserName"].ToString();
                            itemOrder.ItemDescription = sdr["ItemDescription"].ToString();
                            itemOrder.Qty = Convert.ToInt32(sdr["Qty"]);

                            ItemsOrdersList.Add(itemOrder);
                        }
                    }
                }
    
                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return ItemsOrdersList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region BUCKETS
        public static bool CheckThisBucketRelation(int parentID, int childID)
        {
            try
            {
                bool status = false;

                string sqlQry = $"SELECT * FROM tbl_BucketsConfig WHERE ParentID= {parentID} AND ChildID = {childID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        status = true;
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return status;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool InsertThisBucketRelation(int parentID, int childID)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_BucketsConfig (ParentID, ChildID) VALUES ({parentID}, {childID})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false;
            }
        }
        public static List<clsItem> GetBucketsList()
        {
            try
            {
                List<clsItem> bucketsList = new List<clsItem>();

                string sqlQry = "SELECT ID, ItemDescription from tbl_Items WHERE ID IN (SELECT DISTINCT ParentID from tbl_BucketsConfig)";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();

                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.ItemDescription = sdr["ItemDescription"].ToString(); ;
                            bucketsList.Add(item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return bucketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static List<clsItem> GetBucketItemsList(int bucketID)
        {
            try
            {
                List<clsItem> bucketsList = new List<clsItem>();

                string sqlQry = $"SELECT ID, ItemDescription from tbl_Items WHERE ID IN (SELECT ChildID FROM tbl_BucketsConfig WHERE ParentID = {bucketID})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();

                            item.ID = Convert.ToInt32(sdr["ID"]);
                            item.ItemDescription = sdr["ItemDescription"].ToString(); ;
                            bucketsList.Add(item);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return bucketsList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        public static List<clsItem> GetBucketItemsListByTicketNumber(int ID)
        {
            try
            {
                List<clsItem> itemList = new List<clsItem>();

                string sqlQuery = "SELECT SUM(Qty) AS 'Qty', tbl_Items.ItemDescription AS 'ItemDesc' FROM tbl_BucketsDetail " +
                                  "INNER JOIN tbl_Items ON tbl_BucketsDetail.ItemID = tbl_Items.ID " +
                                  "GROUP BY tbl_Items.ItemDescription " +
                                  "ORDER BY tbl_Items.ItemDescription";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQuery, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsItem item = new clsItem();

                            item.ItemDescription = sdr["ItemDesc"].ToString();
                            item.ItemAvailable = Convert.ToInt32(sdr["Qty"]);
                            itemList.Add(item);
                        }
                    }
                }
                return itemList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static bool DeleteThisBucketRelation(int parentID, int childID)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_BucketsConfig WHERE ParentID = {parentID} AND ChildID = {childID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"DeleteBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static bool DeleteThisBucket(int parentID)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_BucketsConfig WHERE ParentID = {parentID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"DeleteBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return false; ;
            }
        }
        #endregion

        #region PROMOTIONS
        public static bool InsertPromotion(int promoType, int parentID, int childID, int qty)
        {
            try
            {
                string sqlQry = $"INSERT INTO tbl_PromoConfig (PromoType, PromoID, ItemID, Qty) VALUES ({promoType}, {parentID}, {childID}, {qty})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false;
            }
        }
        public static bool DeletePromotion(int ID)
        {
            try
            {
                string sqlQry = $"DELETE FROM tbl_PromoConfig WHERE PromoID = {ID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"DeleteBartenderOrder ERROR: {ex}", Logger.Severity.ERROR);
                return false; ;
            }
        }
        public static clsPromoConfig GetPromotion(int promoID)
        {
            try
            {
                clsPromoConfig promo = new clsPromoConfig();

                string sqlQry = $"SELECT * FROM tbl_PromoConfig WHERE PromoID = {promoID}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        promo.ID = Convert.ToInt32(sdr["ID"]);
                        promo.PromoType = Convert.ToInt32(sdr["PromoType"]);
                        promo.PromoID = Convert.ToInt32(sdr["PromoID"]);
                        promo.PromoDescription = GetItemDescriptionByItemID(promo.PromoID);
                        promo.ItemID = Convert.ToInt32(sdr["ItemID"]);
                        promo.PromoItemDescription = GetItemDescriptionByItemID(promo.ItemID);
                        promo.PromoQty = Convert.ToInt32(sdr["Qty"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return promo;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<clsPromoConfig> GetPromotionList(int promoType)
        {
            try
            {
                List<clsPromoConfig> promoList = new List<clsPromoConfig>();

                string sqlQry = $"SELECT * from tbl_PromoConfig WHERE PromoType = {promoType}";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            clsPromoConfig promo = new clsPromoConfig();

                            promo.ID = Convert.ToInt32(sdr["ID"]);
                            promo.PromoType = Convert.ToInt32(sdr["PromoType"]);
                            promo.PromoID = Convert.ToInt32(sdr["PromoID"]);
                            promo.PromoDescription = GetItemDescriptionByItemID(promo.PromoID);
                            promo.ItemID = Convert.ToInt32(sdr["ItemID"]);
                            promo.PromoItemDescription = GetItemDescriptionByItemID(promo.ItemID);
                            promo.PromoQty = Convert.ToInt32(sdr["Qty"]);
                            promoList.Add(promo);
                        }
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return promoList;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        #endregion

        #region DAILYACCOUNTANTREPORT
        public static bool InsertDailyAccountantReport(clsDailyAccountantReport dar)
        {
            try
            {
                string sqlQry = "INSERT INTO tbl_DailyAccountantReport (BussinessDate, GrossSales, NetSales, Sales_Cash, Sales_CreditCard, Sales_Transfer, Sales_Voucher, Drawer_Cash, Drawer_CreditCard, Drawer_Transfer, Drawer_Voucher) " +
                                $"VALUES ('{dar.BussinessDate}', {dar.GrossSales}, {dar.NetSales}, {dar.Sales_Cash}, {dar.Sales_CreditCard}, {dar.Sales_Transfer}, {dar.Sales_Voucher}, {dar.Drawer_Cash}, {dar.Drawer_CreditCard}, {dar.Drawer_Transfer}, {dar.Drawer_Voucher})";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return false;
            }
        }

        public static clsDailyAccountantReport GetDailyAccountantReport(string bussinessDate)
        {
            try
            {
                clsDailyAccountantReport dar = new clsDailyAccountantReport();

                string sqlQry = $"SELECT * FROM tbl_DailyAccountantReport WHERE BussinessDate = '{bussinessDate}'";

                using (sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    sqlConn.Open();
                    sqlCmd = new SqlCommand(sqlQry, sqlConn);
                    SqlDataReader sdr = sqlCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        dar.ID = Convert.ToInt32(sdr["ID"]);
                        dar.BussinessDate = sdr["BussinessDate"].ToString();
                        dar.GrossSales = Convert.ToInt32(sdr["GrossSales"]);
                        dar.NetSales = Convert.ToInt32(sdr["NetSales"]);
                        dar.Sales_Cash = Convert.ToInt32(sdr["Sales_Cash"]);
                        dar.Sales_CreditCard = Convert.ToInt32(sdr["Sales_CreditCard"]);
                        dar.Sales_Transfer = Convert.ToInt32(sdr["Sales_Transfer"]);
                        dar.Sales_Voucher = Convert.ToInt32(sdr["Sales_Voucher"]);
                        dar.Drawer_Cash = Convert.ToInt32(sdr["Drawer_Cash"]);
                        dar.Drawer_CreditCard = Convert.ToInt32(sdr["Drawer_CreditCard"]);
                        dar.Drawer_Transfer = Convert.ToInt32(sdr["Drawer_Transfer"]);
                        dar.Drawer_Voucher = Convert.ToInt32(sdr["Drawer_Voucher"]);
                    }
                }

                if (Settings.Default.DebugTrace)
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, sqlQry, Logger.Severity.DEBUG);

                return dar;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }
        #endregion
    }
}
