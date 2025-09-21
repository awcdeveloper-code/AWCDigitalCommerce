using System;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using File = Google.Apis.Drive.v3.Data.File;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfDriveBackup.xaml
    /// </summary>
    public partial class wpfDriveBackup : Window
    {
        private DispatcherTimer timer;

        private string ConnectionString = Settings.Default.TicketsControllerDbConn;
        private const string DatabaseName = "AWCDigitalCommerce";
        private const string BackupFolder = @"C:\AWC.DigitalCommerce\MSSQL\Backup";
        private const string BackupFileName = "AWCDigitalCommerce.bak";

        public wpfDriveBackup()
        {
            InitializeComponent();

            // Ensure backup folder exists
            Directory.CreateDirectory(BackupFolder);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            // Creates backup
            string FullPathBackup = System.IO.Path.Combine(BackupFolder, BackupFileName);
            BackupDatabase(ConnectionString, DatabaseName, FullPathBackup);
            UploadFileToDriveAsync(FullPathBackup);
        }

        private void BackupDatabase(string connectionString, string databaseName, string backupFilePath)
        {
            string backupQuery = $@"
                BACKUP DATABASE [{databaseName}]
                TO DISK = '{backupFilePath}'
                WITH FORMAT, MEDIANAME = 'DbBackups', NAME = 'Full Backup of {databaseName}';";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(backupQuery, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private async void UploadFileToDriveAsync(string filePath)
        {
            try
            {
                StatusText.Text = "Status: Authenticating...";

                var driveService = await AuthenticateAsync();

                StatusText.Text = "Status: Uploading file...";

                var fileId = await UploadFileAsync(driveService, filePath);

                StatusText.Text = $"Status: Upload completed! File ID: {fileId}";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Error: " + ex.Message;
            }
        }

        private async Task<DriveService> AuthenticateAsync()
        {
            using (var stream = new FileStream(Settings.Default.TicketsControllerCredentials, FileMode.Open, FileAccess.Read))
            {
                // Load client secrets from JSON file
                var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;
                var scopes = new[] { DriveService.Scope.DriveFile };

                // This will open a browser window for user consent on first run
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Drive.Api.Auth.Store"));
                // Create Drive API service.
                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TicketsController",
                });
            }
        }

        private async Task<string> UploadFileAsync(DriveService service, string filePath)
        {
            var fileMetadata = new File()
            {
                Name = Path.GetFileName(filePath)
            };

            // Delete existing file with same name (optional)
            var listRequest = service.Files.List();
            
            listRequest.Q = $"name = '{Path.GetFileName(filePath)}' and trashed = false";
            
            var existingFiles = await listRequest.ExecuteAsync();

            foreach (var file in existingFiles.Files)
            {
                await service.Files.Delete(file.Id).ExecuteAsync();
            }

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
                request.Fields = "id";
                await request.UploadAsync();
                var file = request.ResponseBody;
                return file.Id;
            }
        }
    }
}