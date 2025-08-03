using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int qty)
            {
                if (qty > 50)
                    return Brushes.Green;   // High quantity
                else if (qty > 25)
                    return Brushes.Yellow;  // Medium quantity
                else if (qty > 10)
                    return Brushes.Red;     // Low quantity
            }
            return Brushes.DarkBlue; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
