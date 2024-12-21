using System;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class WeatherCaller
    {
        private readonly WeatherService _weatherService;

        public WeatherCaller(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        public async Task<string> GetWeather(string location)
        {
            try
            {
                string weatherData = await _weatherService.GetWeatherDataAsync(location);
                return weatherData;
            }
            catch (Exception ex)
            {
                return $"500 Internal server error: {ex.Message}";
            }
        }
    }
}
