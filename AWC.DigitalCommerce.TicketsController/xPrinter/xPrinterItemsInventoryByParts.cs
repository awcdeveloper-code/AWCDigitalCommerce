using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterItemsInventoryByParts
    {
        private string type = string.Empty;
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsItem> itemsList = new List<clsItem>();
        private List<clsItem> partItemsList = new List<clsItem>();
        private int step = 0;

        public xPrinterItemsInventoryByParts(List<clsItem> _itemsList, string _type)
        {
            itemsList = _itemsList;
            this.type = _type;
        }

        public void print()
        {
            if (Settings.Default.TicketPrinter.Length == 0) return;

            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();

            PaperSize psize = new PaperSize("Custom", Settings.Default.TicketWidth, Settings.Default.TicketLength);

            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdoc.DefaultPageSettings.PaperSize.Width = Settings.Default.TicketWidth;
            pdoc.DefaultPageSettings.PaperSize.Height = Settings.Default.TicketLength;
            pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.TicketPrinter;

            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

            step = 1;   // print header
            pdoc.Print();

            step = 2;   // print items one by one
            foreach (clsItem item in itemsList)
            {
                partItemsList.Add(item);
                pdoc.Print();
                partItemsList.Clear();
            }

            step = 3;   // print footer
            pdoc.Print();
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen blackPen = new Pen(Color.Black, 4);

            int startX = 0;
            int startY = 0;
            int Offset = 0;
            
            switch (step)
            {
                case 1:
                    // TICKET HEADER
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 20;
                    graphics.DrawString(" ESTADO DEL INVENTARIO", new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 20;
                    graphics.DrawString("       " + type, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 25;

                    // ITEMS HEADER
                    workVar = "PRODUCTO                  DISP";
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 20;

                    //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                    //Offset += 18;

                    graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                    break;
                case 2:
                    // PRINT ITEMS LIST
                    foreach (clsItem item in partItemsList)
                    {
                        workVar = Helper.FormatInventoryLine(item);
                        graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), 0, 0);
                    }
                    break;
                case 3:
                    Offset = 10;
                    //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                    //Offset += 18;

                    graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;

                    graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 75;
                    workVar = ".   .    .    .    .    .    .";
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    break;
            }
        }
    }
}
