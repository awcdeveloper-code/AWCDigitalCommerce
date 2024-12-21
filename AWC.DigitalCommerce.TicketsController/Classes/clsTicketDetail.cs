using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsTicketDetail
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public int ItemID { get; set; }
        public int ItemType { get; set; }
        public int ItemSubType { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int UnitCost { get; set; }
        public int TotalPrice { get; set; }
        public int TotalCost { get; set; }
        public string Note { get; set; }
        public bool Bucket { get; set; }
        public string ImagePath { get; set; }
    }
}
