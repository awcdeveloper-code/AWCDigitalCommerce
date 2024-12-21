using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSQLQuery.xaml
    /// </summary>
    public partial class wpfSQLQuery : Window
    {
        public wpfSQLQuery()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            txtBoxSQLCmd.Focus();
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Clean(object sender, RoutedEventArgs e)
        {
            txtBoxSQLCmd.Text = string.Empty;
            dgvSQLQueryTab.Columns.Clear();
            dgvSQLQueryTab.ItemsSource = null;
            txtBoxSQLCmd.Focus();
        }

        private void btn_Execute(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> sqlErrors = new List<string>();

                if (txtBoxSQLCmd.Text.Length == 0) return;

                dgvSQLQueryTab.Columns.Clear();

                using (SqlConnection sqlConn = new SqlConnection(Settings.Default.TicketsControllerDbConn))
                {
                    SqlCommand myTableCommand = new SqlCommand(txtBoxSQLCmd.Text.ToUpper(), sqlConn);
                    DataTable dt = new DataTable();
                    SqlDataAdapter a = new SqlDataAdapter(myTableCommand);
                    a.Fill(dt);
                    dgvSQLQueryTab.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                wpfMessageBox.Show("Tickets Controller", ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, string.Empty);
            }
        }
        public bool IsSQLQueryValid(string sql, out List<string> errors)
        {
            errors = new List<string>();
            TSql140Parser parser = new TSql140Parser(false);
            TSqlFragment fragment;
            IList<ParseError> parseErrors;

            using (TextReader reader = new StringReader(sql))
            {
                fragment = parser.Parse(reader, out parseErrors);

                if (parseErrors != null && parseErrors.Count > 0)
                {
                    errors = parseErrors.Select(e => e.Message).ToList();
                    return false;
                }
            }
            return true;
        }
    }
}
