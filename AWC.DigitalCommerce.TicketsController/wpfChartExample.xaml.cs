using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfChartExample.xaml
    /// </summary>
    public partial class wpfChartExample : Window
    {
        public ChartValues<double> Values { get; set; }
        public string[] Labels { get; set; }

        public wpfChartExample()
        {
            InitializeComponent();

            // Sample data
            Values = new ChartValues<double> { 6, 10, 14, 8, 12 };
            Labels = new[] { "Ene", "Feb", "Mar", "Abr", "May" };

            // Set the DataContext to enable data binding
            DataContext = this;
        }
    }
}
