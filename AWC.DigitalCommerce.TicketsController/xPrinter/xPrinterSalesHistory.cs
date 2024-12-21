using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterSalesHistory
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string dates = string.Empty;
        private List<clsSalesHistory> salesHistory = new List<clsSalesHistory>();

        public xPrinterSalesHistory()
        {

        }

        public xPrinterSalesHistory(List<clsSalesHistory> _salesHistory, string _dates)
        {
            salesHistory = _salesHistory;
            dates = _dates;
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
                    Offset += 20;
                }

                graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString("HISTORIAL DE VENTAS", new Font("Consolas", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                workVar = " DEL " + DB.ConverTicketDate(dates.Split('|')[0]) + " AL " + DB.ConverTicketDate(dates.Split('|')[1]);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                // ITEMS HEADER
                workVar = "    FECHA        VENTAS";
                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 10;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                // PRINT LIST
                int totSales = 0;
                string saleTot = String.Empty;

                foreach (clsSalesHistory saleHist in salesHistory)
                {
                    totSales += saleHist.salesTotal;
                    saleTot = saleHist.salesTotal.ToString("N0");
                    workVar = " " + saleHist.salesDate + ": " + saleTot.PadLeft(11);
                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                saleTot = totSales.ToString("N0");
                workVar = "      TOTAL: " + saleTot.PadLeft(11);
                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }
    }
}
