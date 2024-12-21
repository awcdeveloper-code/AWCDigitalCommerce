using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfCocktailRecipes.xaml
    /// </summary>
    public partial class wpfCocktailRecipes : Window
    {
        string cocktailName = string.Empty;
        string[] cocktailRecipes = Directory.GetFiles(Settings.Default.CocktailRecipesPath);

        public wpfCocktailRecipes()
        {
            InitializeComponent();

            foreach (string cocktail in cocktailRecipes)
            {
                cocktailName = Path.GetFileNameWithoutExtension(cocktail);
                lBox_CocktailRecipes.Items.Add(cocktailName.ToUpper());
            }
        }

        private void lBox_CocktailRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string txtCocktailRecipeName = lBox_CocktailRecipes.SelectedItem as string;

            foreach (string cocktail in cocktailRecipes)
            {
                if (cocktail.ToUpper().Contains(txtCocktailRecipeName))
                {
                    txtCocktailRecipeContent.Text = File.ReadAllText(cocktail);
                }
            }
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
