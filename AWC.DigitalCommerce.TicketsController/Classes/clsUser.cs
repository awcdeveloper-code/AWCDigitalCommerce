using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsUser
    {
        public int userID { get; set; }
        public DateTime userDTCreation { get; set; }
        public string userPIN { get; set; }
        public string userPW { get; set; }
        public string userName { get; set; }
        public string userAccessLevel { get; set; }
        public bool userActive { get; set; }
        public string userSecurityProfile { get; set; }
        public bool userPowerAdmin { get; set; }
    }
}
