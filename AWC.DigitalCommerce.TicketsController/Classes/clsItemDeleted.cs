using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsItemDeleted
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int Qty { get; set; }
        public int WhoDeleted { get; set; }
        public string WhoDeletedName { get; set; }
        public int WhoAuth { get; set; }
        public string WhoAuthName { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedAtString { get; set; }
    }
}
