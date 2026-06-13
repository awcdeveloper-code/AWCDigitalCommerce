using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.xPrinter
{
    public class xPrinterMealOrder80mm
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string custDesc = string.Empty;
        private List<string> mealList = new List<string>();

        public xPrinterMealOrder80mm()
        {

        }

        public xPrinterMealOrder80mm(string _custDesc, List<string> _mealList)
        {
            custDesc = _custDesc;
            mealList = _mealList;
        }

        public void print()
        {
            if (Settings.Default.KitchenPrinter.Length == 0) return;

            if (mealList.Count > Settings.Default.PrintKitchenOrderHigherThan)
            {
                PrintDialog pd = new PrintDialog();
                pdoc = new PrintDocument();

                PrinterSettings ps = new PrinterSettings();
                PaperSize psize = new PaperSize("Custom", Settings.Default.TicketWidth, Settings.Default.TicketLength);

                pd.Document = pdoc;
                pd.Document.DefaultPageSettings.PaperSize = psize;
                pdoc.DefaultPageSettings.PaperSize.Width = Settings.Default.TicketWidth;
                pdoc.DefaultPageSettings.PaperSize.Height = Settings.Default.TicketLength;
                pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.KitchenPrinter;

                pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

                for (int i = 1; i <= Settings.Default.KitchenPrinterCopies; i++)
                    pdoc.Print();
            }
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;

            const int paperWidth = 300;

            int Offset = 0;

            Pen blackPen = new Pen(Color.Black, 2);

            Brush brush = Brushes.Black;

            //==================================================
            // ENCABEZADO
            //==================================================

            DrawCenteredText(graphics, "ORDEN DE COCINA", new Font("Arial", 16, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 30;

            DrawCenteredText(graphics, Settings.Default.WorkStationType, new Font("Arial", 13, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 25;

            if (custDesc.Length > 30)
                custDesc = custDesc.Substring(0, 30);

            DrawCenteredText(graphics, custDesc, new Font("Arial", 12, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 30;

            graphics.DrawLine(blackPen, 0, Offset, paperWidth, Offset);

            Offset += 10;

            //==================================================
            // FECHA
            //==================================================

            DrawCenteredText(graphics, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), new Font("Arial", 10, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 20;

            //==================================================
            // COLABORADOR
            //==================================================

            clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());

            DrawCenteredText(graphics, $"COLABORADOR: {userProf.userName}", new Font("Arial", 10, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 20;

            graphics.DrawLine(blackPen, 0, Offset, paperWidth, Offset);

            Offset += 15;

            //==================================================
            // PRODUCTOS
            //==================================================

            foreach (string meal in mealList)
            {
                string[] parts = meal.Split('|');

                int qty = Convert.ToInt32(parts[0]);
                string desc = parts[1];
                string notes = parts[2];

                string itemLine = Format80mmItem(qty, desc.ToUpper());

                graphics.DrawString(itemLine, new Font("Arial", 14, FontStyle.Bold), brush, 0, Offset);

                Offset += 25;

                // Observaciones
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    graphics.DrawString(">> " + notes.ToUpper(), new Font("Arial", 11, FontStyle.Italic), brush, 15, Offset);

                    Offset += 22;
                }

                Offset += 5;

                DB.InsertItemOrder(Settings.Default.WhoOpen.ToString(), desc, qty);
            }

            //==================================================
            // PIE
            //==================================================

            Offset += 10;

            graphics.DrawLine(blackPen, 0, Offset, paperWidth, Offset);

            Offset += 20;

            DrawCenteredText(graphics, "*** FIN DE ORDEN ***", new Font("Arial", 10, FontStyle.Bold), brush, Offset, paperWidth);

            Offset += 50;

            DrawCenteredText(graphics, "***", new Font("Arial", 10, FontStyle.Bold), brush, Offset, paperWidth);

            e.HasMorePages = false;
        }

        // Helpers
        private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, int y, int paperWidth)
        {
            SizeF size = g.MeasureString(text, font);
            float x = (paperWidth - size.Width) / 2;
            g.DrawString(text, font, brush, x, y);
        }

        private string Format80mmItem(int qty, string desc)
        {
            return qty.ToString().PadRight(4) + desc;
        }
    }
}
