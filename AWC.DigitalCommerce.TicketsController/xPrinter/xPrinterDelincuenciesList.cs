using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterDelincuenciesList
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsDelincuency> delincuenciesList = new List<clsDelincuency>();

        public xPrinterDelincuenciesList()
        {

        }

        public xPrinterDelincuenciesList(List<clsDelincuency> _delincuenciesList)
        {
            delincuenciesList = _delincuenciesList;
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
            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 40;
            graphics.DrawString("*** CLIENTES MOROSOS ***", new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 30;

            // ITEMS HEADER
            workVar = "CLIENTE FRECUENTE    MOROSIDAD";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 10;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            // PRINT LIST
            string tot = string.Empty;

            int tot_sum_0_8_days = 0;
            int tot_sum_9_15_days = 0;
            int tot_sum_16_30_days = 0;
            int tot_sum_31_45_days = 0;
            int tot_sum_46_60_days = 0;
            int tot_sum_61_days = 0;
            int grlTot = 0;

            foreach (clsDelincuency customer in delincuenciesList)
            {
                graphics.DrawString(customer.CustomerName, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                if (customer.sum_0_8_days > 0)
                {
                    grlTot += customer.sum_0_8_days;
                    tot_sum_0_8_days += customer.sum_0_8_days;
                    tot = customer.sum_0_8_days.ToString("N0");
                    workVar = new string(' ', 11) + "1 - 8 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                if (customer.sum_9_15_days > 0)
                {
                    grlTot += customer.sum_9_15_days;
                    tot_sum_9_15_days += customer.sum_9_15_days;
                    tot = customer.sum_9_15_days.ToString("N0");
                    workVar = new string(' ', 10) + "9 - 15 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                if (customer.sum_16_30_days > 0)
                {
                    grlTot += customer.sum_16_30_days;
                    tot_sum_16_30_days += customer.sum_16_30_days;
                    tot = customer.sum_16_30_days.ToString("N0");
                    workVar = new string(' ', 9) + "16 - 30 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                if (customer.sum_31_45_days > 0)
                {
                    grlTot += customer.sum_31_45_days;
                    tot_sum_31_45_days += customer.sum_31_45_days;
                    tot = customer.sum_31_45_days.ToString("N0");
                    workVar = new string(' ', 7) + "31 - 45 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                if (customer.sum_46_60_days > 0)
                {
                    grlTot += customer.sum_46_60_days;
                    tot_sum_46_60_days += customer.sum_46_60_days;
                    tot = customer.sum_46_60_days.ToString("N0");
                    workVar = new string(' ', 7) + "46 - 60 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                if (customer.sum_61_days > 0)
                {
                    grlTot += customer.sum_61_days;
                    tot_sum_61_days += customer.sum_61_days;
                    tot = customer.sum_61_days.ToString("N0");
                    workVar = new string(' ', 10) + "MÁS 60 DIAS: " + tot.PadLeft(7);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;
                }
                tot = string.Empty;
            }
            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            // SUMMARY
            workVar = new string(' ', 11) + "1 - 8 DIAS: " + tot_sum_0_8_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = new string(' ', 10) + "9 - 15 DIAS: " + tot_sum_9_15_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = new string(' ', 9) + "16 - 30 DIAS: " + tot_sum_16_30_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = new string(' ', 9) + "31 - 45 DIAS: " + tot_sum_31_45_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = new string(' ', 9) + "46 - 60 DIAS: " + tot_sum_46_60_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = new string(' ', 7) + "MÁS DE 60 DIAS: " + tot_sum_61_days.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;

            workVar = "TOTAL MOROSIDAD: " + grlTot.ToString("N0").PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 15;
            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
