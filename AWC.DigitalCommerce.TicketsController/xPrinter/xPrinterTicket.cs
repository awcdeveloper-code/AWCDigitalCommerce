using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using AWC.DigitalCommerce.TicketsController.Controls;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterTicket
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();
        private int step = 0;

        public xPrinterTicket()
        {

        }

        public xPrinterTicket(clsTicketsForDataGrid _ticket, string newName = "")
        {
            ticket = _ticket;

            if (newName.Length > 0)
            {
                ticket.CustomerID = newName;
            }
        }

        public void print()
        {
            try
            {
                if (Settings.Default.TicketPrinter.Length == 0) return;

                PrintDialog pd = new PrintDialog();
                pdoc = new PrintDocument();

                PrinterSettings ps = new PrinterSettings();
                PaperSize psize = new PaperSize("Custom", Settings.Default.TicketWidth, Settings.Default.TicketLength);

                pd.Document = pdoc;
                pd.Document.DefaultPageSettings.PaperSize = psize;
                pdoc.DefaultPageSettings.PaperSize.Width = Settings.Default.TicketWidth;
                pdoc.DefaultPageSettings.PaperSize.Height = Settings.Default.TicketLength;
                pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.TicketPrinter;

                pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
                pdoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                Pen blackPen = new Pen(Color.Black, 4);

                int startX = 0;
                int startY = 0;
                int Offset = 0;

                Pen myPen = new Pen(Color.Black);
                myPen.Width = 2;

                if (Settings.Default.PrintBusinessLogo)
                {
                    Image img = Image.FromFile(Settings.Default.BusinessLogo);

                    int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                    int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                    graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    Offset += LogoHeigh;
                }

                if (Settings.Default.BusinessName.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                }

                if (Settings.Default.BusinessID.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessID), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }

                if (Settings.Default.BusinessPhoneNumber.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessPhoneNumber), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }

                if (Settings.Default.BusinessAddress1.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress1), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }

                if (Settings.Default.BusinessAddress2.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress2), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;
                }
                workVar = "FEC: " + ticket.TicketDate + "  FOLIO: " + ticket.ID.ToString("000000");
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                if (ticket.CustomerID.Length > 22)
                    ticket.CustomerID = ticket.CustomerID.Substring(0, 22);

                graphics.DrawString(new string(' ', 23 - ticket.CustomerID.Length) + ticket.CustomerID, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                clsTicket t = DB.GetTicket(ticket.ID);
                clsUser userProf = Helper.CheckUserProfile(t.WhoOpened.ToString());
                workVar = "ATENDIDO POR: " + userProf.userName;
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                workVar = $"CREADO: {t.CreateAt.ToString("dd-MM-yyyy hh:mm tt")}";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                workVar = "CA DESCRIPCION          PRECIO";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                clsTicket tck = new clsTicket();
                int custID = 0;
                string guid = string.Empty;

                if (ticket.ID > 0)
                {
                    tck = DB.GetTicket(ticket.ID);
                    guid = tck.GUID;
                }
                else
                {
                    custID = DB.GetIDByCustomerID(ticket.CustomerID);
                    guid = DB.GetTicketGUID(Helper.RevertFormatDate(ticket.TicketDate), custID, Convert.ToInt32(ticket.Status));
                }

                List<clsItemDetailForDatagrid> lstItems = DB.GetItemsByGUID(guid, Settings.Default.AllowTicketSummary);

                int totalPrice = 0;
                int totalCash = 0;

                foreach (clsItemDetailForDatagrid itemDet in lstItems)
                {
                    if (itemDet.ItemDesc.Contains("EFECTIVO"))
                    {
                        totalCash += itemDet.TotalPrice;
                        continue;
                    }

                    workVar = Helper.FormatItemDetailLine(itemDet);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                    totalPrice += itemDet.TotalPrice;
                }

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                string subTot = totalPrice.ToString("N0");
                workVar = new string(' ', 13) + "SUBTOTAL: " + subTot.PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                // SERVICE FEE
                string serviceFee = ticket.ServiceFee.ToString("N0");
                workVar = new string(' ', 9) + "10% SERVICIO: " + serviceFee.PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                // IVA Fee
                if (Settings.Default.ATVApplyFee)
                {
                    string ivaFee = ticket.IVAFee.ToString("N0");
                    workVar = new string(' ', 14) + "13% IVA: " + ivaFee.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }

                // CASH IN ADVANCE
                if (totalCash > 0)
                {
                    Offset += 10;
                    workVar = " TOTAL VENTA : " + (totalPrice + ticket.ServiceFee + ticket.IVAFee).ToString("N0").PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;

                    workVar = "MÁS EFECTIVO : " + totalCash.ToString("N0").PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;

                    workVar = "  POR COBRAR : " + (totalPrice + ticket.ServiceFee + +ticket.IVAFee + totalCash).ToString("N0").PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 30;
                }
                else
                {
                    Offset += 12;

                    // TICKET TOTAL
                    string tot = (totalPrice + ticket.ServiceFee + ticket.IVAFee).ToString("N0");
                    graphics.DrawString(new string(' ', 10 - (tot.Length / 2)) + tot, new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 40;

                    // TICKET IN US DOLLARS
                    if (Settings.Default.USDollarExchangeRate > 0)
                    {
                        int totalUSD = (int)Math.Ceiling((double)(totalPrice + ticket.ServiceFee + ticket.IVAFee) / (double)Settings.Default.USDollarExchangeRate);
                        string totUSD = "USD " + (totalUSD).ToString("N0");
                        graphics.DrawString(new string(' ', 23 - (totUSD.Length / 2)) + totUSD, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 20;
                    }
                }

                // PAYMENT MODE
                if (!ticket.Status)
                {
                    string payMethod = string.Empty;

                    if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                        payMethod = "EFECTIVO";

                    if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                        payMethod = "TARJETA DE CRÉDITO";

                    if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                        payMethod = "SINPE";

                    // MIXED PAYMENT
                    if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                    {
                        payMethod = "EFEC+TARJ";
                    }

                    if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                    {
                        payMethod = "EFEC+SINPE";
                    }

                    if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                    {
                        payMethod = "TARJ+SINPE";
                    }

                    if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                    {
                        payMethod = "EFEC+TARJ+SINPE";
                    }

                    Offset += 10;
                    graphics.DrawString(new string(' ', 23 - payMethod.Length) + payMethod, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 30;
                }

                workVar = ticket.Status ? "* PENDIENTE *" : "* CANCELADA *";
                graphics.DrawString(workVar, new Font("Arial Narrow", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 40;

                workVar = "* GRACIAS POR SU VISITA *";
                graphics.DrawString(workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                if (!ticket.Status && Settings.Default.PrintTicketFooter)
                {
                    graphics.DrawString("ESTE NEGOCIO ESTÁ ADSCRITO AL", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString("   RÉGIMEN DE TRIBUTACIÓN", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString("SIMPLIFICADA PARA COMERCIANTES", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString("MINORISTAS Y BARES N° 25514-H", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;

                    graphics.DrawString("*NO EMITE FACTURA ELECTRÓNICA*", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;

                    graphics.DrawString("AUTORIZADO MEDIANTE RESOLUCIÓN", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString("  NÚMERO DG T-R-033.2019 DEL", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString("    20 DE JUNIO 2019 DGTD", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;
                }

                graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                graphics.DrawString(Helper.FormatGralLine(Environment.MachineName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                graphics.DrawString(Helper.FormatGralLine($"© 2021 - {DateTime.Now.ToString("yyyy")} AIDAWARE"), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                // Cut line
                workVar = ".   .    .    .    .    .    .";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
