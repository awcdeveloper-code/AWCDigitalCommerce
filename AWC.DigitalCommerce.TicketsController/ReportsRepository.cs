using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using AWC.DigitalCommerce.TicketsController.Properties;
using SwiftExcel;

namespace AWC.DigitalCommerce.TicketsController
{
    public class ReportsRepository
    {
        private static string GetReportsRepository()
        {
            string GetReportsRepository = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reportes AWC");

            if (!Directory.Exists(GetReportsRepository))
                Directory.CreateDirectory(GetReportsRepository);

            return GetReportsRepository;
        }

        public static void DailyClosing(string workDay, List<clsTicketsForDataGrid> ticketsList)
        {
            try
            {
                string outputPath = GetReportsRepository() + @"\\" + DB.ConverTicketDate(Settings.Default.BusinessDate) + "-DailyClosing.csv";

                using (StreamWriter sw = new StreamWriter(outputPath, false))
                {
                    // print header
                    sw.WriteLine("DAILY CLOUSURE");
                    sw.WriteLine("WORK DAY: " + workDay.Substring(6,2) + "." + workDay.Substring(4, 2) + "." + workDay.Substring(0, 4));
                    sw.WriteLine();
                    sw.WriteLine("TICKET;PAYMENT METHOD;TOTAL");

                    int pendiente = 0;
                    int efectivo = 0;
                    int tarjCred = 0;
                    int transSinpe = 0;
                    int tot = 0;

                    // print tickets
                    foreach (clsTicketsForDataGrid ticket in ticketsList)
                    {
                        string ticketNum = string.Empty;
                        string payMethod = string.Empty;
                        string total = string.Empty;

                        ticketNum = "'" + ticket.ID.ToString("000000");

                        switch (ticket.PayMethod)
                        {
                            case 0:
                                pendiente += ticket.TotalPrice;
                                payMethod = "PENDIENTE";
                                break;
                            case 1:
                                // SINGLE
                                if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                                    payMethod = "EFECTIVO";
                                if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                                    payMethod = "TARJ CRED";
                                if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                                    payMethod = "TRANS SINPE";
                                // MULTI
                                if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                                    payMethod = "EFEC+TARJ";
                                if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                                    payMethod = "EFEC+TRAN";
                                if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                                    payMethod = "TARJ+TRAN";
                                if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                                    payMethod = "EFEC+TARJ+TRAN";

                                efectivo += ticket.Cash;
                                tarjCred += ticket.CreditCard;
                                transSinpe += ticket.Transfer;
                                break;
                        }
                        total = ticket.TotalPrice.ToString("N0");
                        sw.WriteLine(ticketNum + ";" + payMethod + ";" + total.PadLeft(7));
                    }

                    // print footer
                    sw.WriteLine();
                    sw.WriteLine(";PENDIENTE:;" + pendiente.ToString("N0"));
                    sw.WriteLine(";EFECTIVO:;" + efectivo.ToString("N0"));
                    sw.WriteLine(";TARJ CRED:;" + tarjCred.ToString("N0"));
                    sw.WriteLine(";SINPE:;" + transSinpe.ToString("N0"));
                    sw.WriteLine();
                    tot = pendiente + efectivo + tarjCred + transSinpe;
                    sw.WriteLine(";TOTAL:;" + tot.ToString("N0"));
                }

                // generate Excel
                DailyClosingXLS(workDay, ticketsList);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        
        public static void DailyClosingXLS(string workDay, List<clsTicketsForDataGrid> ticketsList)
        {
            try
            {
                // create MSExcel Object
                Application xlsApp = new Application();

                if (xlsApp == null)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Excel is not properly installed!", Logger.Severity.ERROR);
                    Helper.ShowMessage("Excel is not properly installed!", System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                // open MSExcel Template
                Workbook xlsWorkBook = xlsApp.Workbooks.Add(@"C:\AWC.DigitalCommerce\Templates\AWC_DailyClosingTemplate");
                Worksheet xlsWorkSheet = (Worksheet)xlsWorkBook.Worksheets.get_Item(1);

                // update worksheet name
                xlsWorkSheet.Name = "Daily Closing - " + workDay.Substring(6, 2) + "-" + workDay.Substring(4, 2) + "-" + workDay.Substring(0, 4);

                // add tickets
                int pendiente = 0;
                int efectivo = 0;
                int tarjCred = 0;
                int transSinpe = 0;

                // print tickets, starting row 3
                int row = 3;
                foreach (clsTicketsForDataGrid ticket in ticketsList)
                {
                    string payMethod = string.Empty;

                    switch (ticket.PayMethod)
                    {
                        case 0:
                            pendiente += ticket.TotalPrice;
                            payMethod = "PENDING TO PAY";
                            break;
                        case 1:
                            // SINGLE
                            if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                                payMethod = "CASH";
                            if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                                payMethod = "CREDIT CARD";
                            if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                                payMethod = "SINPE TRANSFER";
                            // MULTI
                            if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                                payMethod = "CASH & CREDIT CARD";
                            if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                                payMethod = "CASH & SINPE TRANSFER";
                            if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                                payMethod = "CREDIT CARD + SINPE TRANSFER";
                            if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                                payMethod = "CASH & CREDIT CARD & SINPE TRANSFER";

                            efectivo += ticket.Cash;
                            tarjCred += ticket.CreditCard;
                            transSinpe += ticket.Transfer;
                            break;
                    }

                    xlsWorkSheet.Cells[row, 1] = ticket.ID;
                    xlsWorkSheet.Cells[row, 2] = payMethod;
                    xlsWorkSheet.Cells[row, 3] = ticket.TotalPrice;

                    row++;
                }

                // print footer
                row++;
                xlsWorkSheet.Cells[row, 2] = "PENDING:";
                xlsWorkSheet.Cells[row, 3] = pendiente;
                row++;
                xlsWorkSheet.Cells[row, 2] = "CASH:";
                xlsWorkSheet.Cells[row, 3] = efectivo;
                row++;
                xlsWorkSheet.Cells[row, 2] = "CREDIT CARD:";
                xlsWorkSheet.Cells[row, 3] = tarjCred;
                row++;
                xlsWorkSheet.Cells[row, 2] = "SINPE TRANSFER:";
                xlsWorkSheet.Cells[row, 3] = transSinpe;
                row++;
                xlsWorkSheet.Cells[row, 2] = "TOTAL:";
                xlsWorkSheet.Cells[row, 3] = pendiente + efectivo + tarjCred + transSinpe;

                // save and close XML
                xlsWorkBook.SaveAs(DateTime.Now.ToString("yyyy.MM.dd") + "-DailyClosing.xlsx");
                xlsWorkBook.Close();

                // dispose COM objects
                Marshal.ReleaseComObject(xlsWorkSheet);
                Marshal.ReleaseComObject(xlsWorkBook);
                Marshal.ReleaseComObject(xlsApp);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        public static void InventoryStatusXLS (string workDay, List<clsItem> itemsList)
        {
            string excelFileName = string.Empty;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // create MSExcel Object
                Application xlsApp = new Application();

                if (xlsApp == null)
                {
                    Logger.WriteToLog("InventoriesManagement", "Excel is not properly installed!", Logger.Severity.ERROR);
                    Helper.ShowMessage("Excel is not properly installed!", System.Windows.Forms.MessageBoxIcon.Error);
                }

                // open MSExcel Template
                Workbook xlsWorkBook = xlsApp.Workbooks.Add(@"C:\AWC.DigitalCommerce\Templates\AWC_InventoryStatusTemplate");
                Worksheet xlsWorkSheet = (Worksheet)xlsWorkBook.Worksheets.get_Item(1);

                // update header
                string xlsHeader = "ESTADO DEL INVENTARIO";
                xlsWorkSheet.Name = xlsHeader;
                xlsWorkSheet.Cells[1, 1] = Settings.Default.BusinessName;
                xlsWorkSheet.Cells[2, 1] = xlsHeader;

                // generate the content here
                int row = 4;
                foreach (clsItem item in itemsList)
                {
                    xlsWorkSheet.Cells[row, 1] = item.ID;
                    xlsWorkSheet.Cells[row, 2] = item.ItemDescription;
                    xlsWorkSheet.Cells[row, 3] = item.ItemAvailable;
                    xlsWorkSheet.Cells[row, 4] = item.ItemSold;
                    xlsWorkSheet.Cells[row, 5] = item.ItemDefective;
                    xlsWorkSheet.Cells[row, 6] = item.DebitNotes;
                    xlsWorkSheet.Cells[row, 7] = item.CreditNotes;
                    xlsWorkSheet.Cells[row, 8] = item.ItemMinimum;
                    xlsWorkSheet.Cells[row, 9] = item.ItemParent;
                    xlsWorkSheet.Cells[row, 10] = DB.GetItemDescriptionByItemID(item.ItemParent);
                    row++;
                }

                // save and close XML
                excelFileName = Path.Combine(GetReportsRepository(), DateTime.Now.ToString("dd.MM.yyyy_HH.mm") + "-InventoryStatus.xlsx");
                xlsWorkBook.SaveAs(excelFileName);
                xlsWorkBook.Close();

                // dispose COM objects
                Marshal.ReleaseComObject(xlsWorkSheet);
                Marshal.ReleaseComObject(xlsWorkBook);
                Marshal.ReleaseComObject(xlsApp);

                // generate PDF
                string pdfFileName = GetReportsRepository() + @"\\" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm") + "-InventoryStatus.pdf";
                InventoryStatusPDF(excelFileName, pdfFileName);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public static bool InventoryStatusPDF (string excelDocName, string xpsDocName)
        {
            Application excelApplication = new Application();
            Workbook excelWorkbook = excelApplication.Workbooks.Open(excelDocName);

            try
            {
                excelWorkbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, Filename: xpsDocName, OpenAfterPublish: false);
                excelApplication.Quit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
                return false;
            }
        }

        public static void InventoryStatusSwiftExcel(string workDay, List<clsItem> itemsList)
        {
            string excelFileName = string.Empty;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                excelFileName = Path.Combine(GetReportsRepository(), DateTime.Now.ToString("dd.MM.yyyy_HH.mm") + "-InventoryStatusWithSwiftExcel.xlsx");

                using (ExcelWriter ew = new ExcelWriter(excelFileName))
                {
                    // title
                    ew.Write(Settings.Default.BusinessName, 1, 1);
                    ew.Write("ESTADO DEL INVENTARIO @ " + workDay.Substring(6, 2) + "-" + workDay.Substring(4, 2) + "-" + workDay.Substring(0, 4), 1, 2);

                    // header
                    ew.Write("CÓDIGO", 1, 3);
                    ew.Write("DESCRIPCIÓN DEL PRODUCTO", 2, 3);
                    ew.Write("COSTO", 3, 3);
                    ew.Write("TOTAL COSTO", 4, 3);
                    ew.Write("PRECIO", 5, 3);
                    ew.Write("TOTAL PRECIO", 6, 3);
                    ew.Write("DISPONIBLE", 7, 3);
                    ew.Write("VENDIDO", 8, 3);
                    ew.Write("DAÑADO", 9, 3);
                    ew.Write("DÉBITOS", 10, 3);
                    ew.Write("CRÉDITOS", 11, 3);
                    ew.Write("MÍNIMO", 12, 3);
                    ew.Write("PARIENTE", 13, 3);
                    ew.Write("DESCRIPCIÓN DEL PARIENTE", 14, 3);

                    // generate the content
                    int row = 4;
                    foreach (clsItem item in itemsList)
                    {
                        ew.Write(item.ID.ToString(), 1, row);
                        ew.Write(item.ItemDescription, 2, row);
                        ew.Write(item.UnitCost.ToString("N0"), 3, row);
                        ew.Write((item.UnitCost * item.ItemAvailable).ToString("N0"), 4, row);
                        ew.Write(item.UnitPrice.ToString("N0"), 5, row);
                        ew.Write((item.UnitPrice * item.ItemAvailable).ToString("N0"), 6, row);
                        ew.Write(item.ItemAvailable.ToString(), 7, row);
                        ew.Write(item.ItemSold.ToString(), 8, row);
                        ew.Write(item.ItemDefective.ToString(), 9, row);
                        ew.Write(item.DebitNotes.ToString(), 10, row);
                        ew.Write(item.CreditNotes.ToString(), 11, row);
                        ew.Write(item.ItemMinimum.ToString(), 12, row);
                        ew.Write(item.ItemParent.ToString(), 13, row);
                        ew.Write(DB.GetItemDescriptionByItemID(item.ItemParent), 142, row);
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

    }
}
