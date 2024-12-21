using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterBeveragesSummary
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsItemDetailForDatagrid> mealList = new List<clsItemDetailForDatagrid>();

        public xPrinterBeveragesSummary()
        {

        }

        public xPrinterBeveragesSummary(List<clsItemDetailForDatagrid> _mealList)
        {
            mealList = _mealList;
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
            graphics.DrawString(new string(' ', 12) + DB.ConverTicketDate(Settings.Default.BusinessDate), new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            graphics.DrawString(new string(' ', 2) + "CIERRE DE BEBIDAS", new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            // ITEMS HEADER
            workVar = "CAN DESCRIPCION          TOTAL";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 10;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            int totMeals = 0;
            int totalPrice = 0;
            double reduce10percent = 0;

            // LIST OF MEALS
            foreach (clsItemDetailForDatagrid meal in mealList)
            {
                totMeals += meal.Qty;
                totalPrice += meal.TotalCost;
                workVar = Helper.FormatMealItemDetailLineSummary(meal);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;
            }

            // FOOTER
            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            string tot = totalPrice.ToString("N0");
            workVar ="CANT: " + totMeals.ToString().PadLeft(3) + new string(' ', 7) + "TOTAL: " + tot.PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
