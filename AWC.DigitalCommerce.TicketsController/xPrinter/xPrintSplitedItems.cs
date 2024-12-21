using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrintSplitedItems
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private clsTicket ticket = new clsTicket();
        private List<clsTicketDetail> splitItems = new List<clsTicketDetail>();
        private string customerName = string.Empty;

        public xPrintSplitedItems()
        {

        }

        public xPrintSplitedItems(clsTicket _ticket, List<clsTicketDetail> _splitItems, string _customerName)
        {
            ticket = _ticket;
            splitItems = _splitItems;
            customerName = _customerName;
        }

        public void print()
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

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                Pen blackPen = new Pen(Color.Black, 4);

                int startX = 0;
                int startY = 0;
                int Offset = 0;

                // PRINT LOGO
                Image img = Image.FromFile(Settings.Default.BusinessLogo);

                int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                Pen myPen = new Pen(Color.Black);
                myPen.Width = 2;

                graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                Offset += LogoHeigh;

                // TICKET HEADER
                if (Settings.Default.BusinessName.Length > 0)
                {
                    workVar = new string(' ', 12 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
                    graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 35;
                }

                if (Settings.Default.BusinessID.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessID), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessPhoneNumber.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessPhoneNumber), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessAddress1.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress1), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessAddress2.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress2), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 30;
                }

                // DATE + INVOICE NUMBER
                workVar = "FEC: " + DB.ConverTicketDate(Settings.Default.BusinessDate) + "  FOLIO: " + ticket.ID.ToString("000000");
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                // CUSTOMER
                workVar = "CUENTA SEPARADA";
                graphics.DrawString(new string(' ', 8) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                if (customerName.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(customerName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;
                }
                // ITEMS HEADER
                workVar = "CA DESCRIPCION          PRECIO";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                //Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 10;

                int subTotal = 0;

                // LIST OF ITEMS
                foreach (clsTicketDetail itemDet in splitItems)
                {
                    subTotal += itemDet.TotalPrice;
                    workVar = Helper.FormatSplitItemDetailLine(itemDet);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }

                // TICKET SUB-TOTAL
                //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                //Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 10;

                workVar = new string(' ', 13) + "SUBTOTAL: " + subTotal.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                // SERVICE FEE
                workVar = new string(' ', 9) + "10% SERVICIO: " + ticket.ServiceFee.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                if (Settings.Default.ATVApplyFee)
                {
                    workVar = new string(' ', 14) + "13% IVA: " + ticket.IVAFee.ToString("N0").PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 30;
                }

                // TICKET TOTAL
                string tot = ticket.TotalPrice.ToString("N0");
                graphics.DrawString(new string(' ', 10 - (tot.Length / 2)) + tot, new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 40;

                // TICKET IN US DOLLARS
                if (Settings.Default.USDollarExchangeRate > 0)
                {
                    int totalUSD = (int)Math.Ceiling((double)(ticket.TotalPrice) / (double)Settings.Default.USDollarExchangeRate);
                    string totUSD = "USD " + (totalUSD).ToString("N0");
                    graphics.DrawString(new string(' ', 23 - (totUSD.Length / 2)) + totUSD, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 40;
                }

                graphics.DrawString("PREFACTURA", new Font("Arial Narrow", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 60;

                workVar = "* GRACIAS POR SU VISITA *";
                graphics.DrawString(workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 60;
                // Cut line
                workVar = ".   .    .    .    .    .    .";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}