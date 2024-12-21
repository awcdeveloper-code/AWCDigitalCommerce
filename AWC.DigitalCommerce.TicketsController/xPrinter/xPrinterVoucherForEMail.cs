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

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterVoucherForEMail
    {
        private string workVar = string.Empty;
        private clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

        public xPrinterVoucherForEMail()
        {

        }

        public xPrinterVoucherForEMail(clsTicketsForDataGrid _ticket)
        {
            ticket = _ticket;
        }

        public void print()
        {
            if (Settings.Default.TicketPrinter.Length == 0) return;

            try
            {
                // LIST OF ITEMS
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

                int ticketWidth = Settings.Default.TicketWidth;
                int ticketHeight = 1025;

                using (Bitmap bitmap = new Bitmap(ticketWidth, ticketHeight))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Rectangle rect = new Rectangle(0, 0, ticketWidth, ticketHeight);
                    graphics.FillRectangle(new SolidBrush(Color.White), rect);

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
                        Offset += 30;
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

                    // DATE + INVOICE NUMBER
                    workVar = "FEC: " + ticket.TicketDate + "  FOLIO: " + ticket.ID.ToString("000000");
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 25;

                    // CUSTOMER
                    if (ticket.CustomerID.Length > 22)
                        ticket.CustomerID = ticket.CustomerID.Substring(0, 22);

                    graphics.DrawString(new string(' ', 23 - ticket.CustomerID.Length) + ticket.CustomerID, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 35;

                    // MEAL SERVICE LOGO
                    Image img2 = Image.FromFile(Settings.Default.FoodServiceLogo);
                    graphics.DrawImage(img2, 93 - (img2.Width / 2), startY + Offset);
                    Offset += img2.Height + 20;

                    // PRINT TOTAL PRICE
                    string tot = ticket.TotalPrice.ToString("N0");
                    graphics.DrawString(new string(' ', 10 - (tot.Length / 2)) + tot, new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 50;

                    // TICKET IN US DOLLARS
                    if (Settings.Default.USDollarExchangeRate > 0)
                    {
                        int totalUSD = (int)Math.Ceiling((double)(ticket.TotalPrice) / (double)Settings.Default.USDollarExchangeRate);
                        string totUSD = "USD " + (totalUSD).ToString("N0");
                        graphics.DrawString(new string(' ', 23 - (totUSD.Length / 2)) + totUSD, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 40;
                    }

                    // PAYMENT MODE
                    if (!ticket.Status)
                    {
                        string payMethod = string.Empty;

                        if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                            payMethod = "EFECTIVO";

                        if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                            payMethod = "TARJETA CREDITO";

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
                        graphics.DrawString("PAGO: " + payMethod, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 30;
                    }

                    workVar = ticket.Status ? "PREFACTURA" : "*CANCELADA*";
                    graphics.DrawString(workVar, new Font("Arial Narrow", 18), new SolidBrush(Color.Black), startX, startY + Offset);
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
                    }

                    string saveTicketToFile = Path.Combine(Settings.Default.SerilogRootPath, ticket.ID.ToString("000000") + ".bmp");

                    if (File.Exists(saveTicketToFile))
                    {
                        File.Delete(saveTicketToFile);
                    }

                    bitmap.Save(saveTicketToFile);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
