using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterTicketsPerCustomer
    {
        private string custName = string.Empty;
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsTicketsForDataGrid> ticketsList = new List<clsTicketsForDataGrid>();

        public xPrinterTicketsPerCustomer()
        {

        }

        public xPrinterTicketsPerCustomer(string _custName, List<clsTicketsForDataGrid> _ticketsList)
        {
            custName = _custName;
            ticketsList = _ticketsList;
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
            Graphics graphics = e.Graphics;

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

            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 30;
            graphics.DrawString("  CUENTAS PENDIENTES DE PAGO", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;
            graphics.DrawString(Helper.FormatGralLine(custName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 30;
            workVar = "   FECHA     CUENTA      MONTO";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 10;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            int grlTot = 0;

            foreach (clsTicketsForDataGrid tck in ticketsList)
            {
                grlTot += tck.TotalPrice;
                workVar = tck.TicketDate + "   " + tck.ID.ToString("000000") + "    " + tck.TotalPrice.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;
            }

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;
            workVar = new string(' ', 7) + "TOTAL ADEUDADO: " + grlTot.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 50;
            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
