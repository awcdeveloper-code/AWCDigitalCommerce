using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsRomanYearConverter
    {
        public static string ToRoman(int year)
        {
            var map = new (int Value, string Symbol)[]
            {
                (1000, "M"),
                (900,  "CM"),
                (500,  "D"),
                (400,  "CD"),
                (100,  "C"),
                (90,   "XC"),
                (50,   "L"),
                (40,   "XL"),
                (10,   "X"),
                (9,    "IX"),
                (5,    "V"),
                (4,    "IV"),
                (1,    "I")
            };

            var result = new StringBuilder();

            foreach (var (value, symbol) in map)
            {
                while (year >= value)
                {
                    result.Append(symbol);
                    year -= value;
                }
            }

            return result.ToString();
        }
    }
}
