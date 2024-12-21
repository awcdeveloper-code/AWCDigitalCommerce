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
    public partial class wpfWorkLog : Window
    {
        private Dictionary<string, string> logFilesPath = new Dictionary<string, string>();

        public wpfWorkLog()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            LoadLogTreeView();
        }

        private void LoadLogTreeView()
        {
            try
            {
                foreach (string moduleName in Directory.GetDirectories(Settings.Default.SerilogRootPath))
                {
                    if (moduleName.Contains(" ")) continue;

                    TreeViewItem newModule = new TreeViewItem();
                    newModule.Header = Path.GetFileName(moduleName);
                    newModule.Name = Path.GetFileName(moduleName.Trim());

                    foreach(string fileName in Directory.GetFiles(moduleName, "*.Log"))
                    {
                        TreeViewItem newFileName = new TreeViewItem();
                        newFileName.Header = Path.GetFileName(fileName);
                        newModule.Items.Add(newFileName);

                        logFilesPath.Add(Path.GetFileName(fileName), fileName);
                    }

                    LogTreeView.Items.Add(newModule);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LogTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogContent.Items.Clear();

            TreeViewItem selected = (TreeViewItem) LogTreeView.SelectedItem;
            string fileName = selected.Header.ToString();

            string filePath = string.Empty;

            if (!logFilesPath.TryGetValue(fileName, out filePath))
            {
                return;
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    LogContent.Items.Add(line);
                }
            }
        }
    }
}
