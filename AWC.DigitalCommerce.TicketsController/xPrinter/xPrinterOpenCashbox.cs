using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterOpenCashbox
    {
        private PrintDocument pdoc = null;

        public xPrinterOpenCashbox()
        {

        }

        public void print()
        {
            if (Settings.Default.TicketPrinter.Length == 0) return;

            PaperSize psize = new PaperSize("Custom", 10, 10);

            pdoc = new PrintDocument();

            PrintDialog pd = new PrintDialog();
            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;

            pdoc.DefaultPageSettings.PaperSize.Width = 10;
            pdoc.DefaultPageSettings.PaperSize.Height = 10;
            pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.TicketPrinter;
            //pdoc.PrinterSettings.PrintToFile = true;
            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
            pdoc.Print();
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                graphics.DrawString(".", new Font("Consolas Bold", 6), new SolidBrush(Color.Gray), 0, 0);
                DB.InsertMoneyDrawerLog();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
