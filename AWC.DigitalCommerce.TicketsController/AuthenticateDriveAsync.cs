using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AWC.DigitalCommerce.TicketsController
{
    public class DriveAuthenticator
    {
        public async Task<DriveService> AuthenticateDriveAsync()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = "aidawareconsultancies.apps.googleusercontent.com",
                ClientSecret = "ucfyocmdgujnhtrm"
            };

            var scopes = new[] { DriveService.Scope.Drive };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("Drive.Api.Auth.Store")
            );

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "TicketsController"
            });
        }
    }
}
