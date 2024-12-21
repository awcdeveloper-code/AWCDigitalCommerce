using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsNoteDetail
    {
        public int ID { get; set; }
        public string NoteGUID { get; set; }
        public int ItemType { get; set; }
        public int ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQty { get; set; }
        public int ItemPrice { get; set; }
        public int ItemTotal { get; set; }
    }
}
