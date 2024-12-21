using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfMealNote.xaml
    /// </summary>
    public partial class wpfMealNote : Window
    {
        private Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
        public string mealNote = string.Empty;

        public wpfMealNote(string _itemDesc)
        {
            this.Topmost = true;

            InitializeComponent();

            MealDesc.Content = _itemDesc;

            txtMealNote.Focus();
        }

        private void txtMealNote_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                mealNote = txtMealNote.Text.ToUpper();
                this.Close();
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            mealNote = txtMealNote.Text.ToUpper();
            this.Close();
        }

        private void txtMealNote_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!regex.IsMatch(e.Text))
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }
    }
}
