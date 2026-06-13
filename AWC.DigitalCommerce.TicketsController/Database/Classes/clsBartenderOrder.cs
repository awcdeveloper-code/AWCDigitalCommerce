using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsBartenderOrder
    {
        public string GUID { get; set; }
        public string CustomerID { get; set; }
        public string BeveragesList { get; set; }
        public clsBartenderOrder()
        {
            GUID = string.Empty;
            CustomerID = string.Empty;
            BeveragesList = string.Empty;
        }
    }
}
