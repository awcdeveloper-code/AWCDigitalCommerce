using System;
using System.Net.Http;
using System.Threading.Tasks;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Settings.Default.NwsApiBaseUrl);
        }

        public async Task<string> GetWeatherDataAsync(string location)
        {
            string apiUrl = $"/gridpoints/{location}/forecast";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Failed to fetch weather data: {response.StatusCode}");
            }
        }
    }
}
