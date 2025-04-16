using System.IO;
using System.Windows.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucCocktails : UserControl
    {
        string cocktailName = string.Empty;
        string[] cocktailRecipes = Directory.GetFiles(Settings.Default.CocktailRecipesPath);

        public ucCocktails()
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
    }
}
