using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterItemsPriceList
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsItem> itemsPriceList = new List<clsItem>();

        public xPrinterItemsPriceList()
        {

        }

        public xPrinterItemsPriceList(List<clsItem> _itemsPricelList)
        {
            itemsPriceList = _itemsPricelList;
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
            Rectangle rect = new Rectangle(97 - (img.Width / 2), 0, img.Width, img.Height);
            graphics.DrawImage(img, rect);
            Offset = img.Height;

            // TICKET HEADER
            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 40;
            graphics.DrawString("* CATÁLOGO DE PRECIOS *", new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            // ITEMS HEADER
            workVar = "DESCRIPCIÓN             PRECIO";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 10;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            // PRINT ITEMS LIST
            int type = 0;
            string itemTypeDesc = string.Empty;

            foreach (clsItem item in itemsPriceList)
            {
                if (item.ItemType > type)
                {
                    switch(item.ItemType)
                    {
                        case 1:
                            itemTypeDesc = "CERVEZA/LIGAS";
                            break;
                        case 2:
                            itemTypeDesc = "LICORES";
                            break;
                        case 3:
                            itemTypeDesc = "COMIDAS";
                            break;
                    }

                    type = item.ItemType;

                    graphics.DrawString(itemTypeDesc, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 15;
                    graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset = Offset + 15;
                }

                workVar = Helper.FormatItemPriceLine(item);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset = Offset + 15;
            }
            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
