using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class Cocktail
    {
        public string StrDrink { get; set; }
        public string StrInstructions { get; set; }
        public string[] StrIngredients { get; set; }
    }

    public class CocktailWebService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task GetRecipe(string cocktailName)
        {
            var recipes = await GetCocktailRecipesAsync(cocktailName);

            if (recipes != null)
            {
                foreach (var recipe in recipes)
                {
                    Console.WriteLine($"Drink: {recipe.StrDrink}");
                    Console.WriteLine($"Instructions: {recipe.StrInstructions}");
                    Console.WriteLine("Ingredients:");

                    foreach (var ingredient in recipe.StrIngredients)
                    {
                        if (!string.IsNullOrEmpty(ingredient))
                        {
                            Console.WriteLine($"- {ingredient}");
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"No recipes found for '{cocktailName}'.");
            }
        }

        public static async Task<Cocktail[]> GetCocktailRecipesAsync(string cocktailName)
        {
            try
            {
                string url = $"https://www.thecocktaildb.com/api/json/v1/1/search.php?s={cocktailName}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var cocktailData = JsonSerializer.Deserialize<dynamic>(jsonResponse, options);

                if (cocktailData?.drinks != null)
                {
                    var drinks = cocktailData.drinks;

                    var recipes = new Cocktail[drinks.Length];

                    for (int i = 0; i < drinks.Length; i++)
                    {
                        var drink = drinks[i];
                        var ingredients = new string[15];

                        for (int j = 1; j <= 15; j++)
                        {
                            var ingredient = drink.GetProperty($"strIngredient{j}").GetString();
                            ingredients[j - 1] = ingredient;
                        }

                        recipes[i] = new Cocktail
                        {
                            StrDrink = drink.GetProperty("strDrink").GetString(),
                            StrInstructions = drink.GetProperty("strInstructions").GetString(),
                            StrIngredients = ingredients
                        };
                    }

                    return recipes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
