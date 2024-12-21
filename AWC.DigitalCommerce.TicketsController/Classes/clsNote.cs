using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsNote
    {
        public int ID { get; set; }
        public string NoteDate { get; set; }
        public int NoteType { get; set; }
        public string NoteDescription { get; set; }
        public int NoteAmount { get; set; }
        public string NoteGUID { get; set; }
    }
}
