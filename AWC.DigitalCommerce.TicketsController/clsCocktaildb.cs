using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsCocktaildb
    {
        public static async Task<string> GetCocktailRecipeByName(string cocktailName)
        {
            string apiUrl = $"www.thecocktaildb.com/api/json/v1/1/search.php?s={cocktailName.ToUpper()}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    return $"ERROR: {e.Message}";
                }
            }
        }
    }
}
