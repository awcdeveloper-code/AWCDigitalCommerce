using System;
using System.EnterpriseServices;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Properties;
using Newtonsoft.Json;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public class JSON
    {
        public static async Task ATVSendWebServiceCall(int ticketNumber, string jsonData)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ATVResponse atvdeserialized = new ATVResponse();

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(Settings.Default.ATVEndpointToSend, content);

                // Check the response from the server
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    atvdeserialized = JsonConvert.DeserializeObject<ATVResponse>(responseContent);
                    Console.WriteLine("Request successful!");
                    Console.WriteLine("Response: " + responseContent);

                }
                else
                {
                    Console.WriteLine("Request failed. Status code: " + response.StatusCode);
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + errorContent);
                }
                DB.SetATVStatus(ticketNumber, atvdeserialized, 1);
            }
            Mouse.OverrideCursor = null;
        }

        public static async Task ATVQueryWebServiceCall(int ticketNumber, string jsonData)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ATVResponse atvdeserialized = new ATVResponse();

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(Settings.Default.ATVEndpointToQuery, content);

                // Check the response from the server
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    atvdeserialized = JsonConvert.DeserializeObject<ATVResponse>(responseContent);
                    Console.WriteLine("Request successful!");
                    Console.WriteLine("Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine("Request failed. Status code: " + response.StatusCode);
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + errorContent);
                }

                atvdeserialized.estado = atvdeserialized.estado.ToUpper();

                DB.SetATVStatus(ticketNumber, atvdeserialized, 2);

                Mouse.OverrideCursor = null;

                wpfMessageBox.Show("Ticket Controller", $"ESTADO ACTUAL: {atvdeserialized.estado}", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
            }
        }
    }
}
