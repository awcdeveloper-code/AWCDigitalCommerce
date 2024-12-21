using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class Constants
    {
        public class Titles
        {
            public const string LONGAPPTITLE = "AWC Tickets Controller";
            public const string SHORTGAPPTITLE = "TicketsController";
        }

        public class HTML
        {
            public const string HEADER = "<!DOCTYPE html><html><head><style>table { font-family: arial, sans-serif; border-collapse: collapse; width: 100%;} td, th { border: 1px solid #dddddd; text-align: left; padding: 8px;}tr:nth-child(even) { background-color: #dddddd;}</style></head>";
            public const string BODY = "<body>{0}</body></html>";
        }
    }
}
