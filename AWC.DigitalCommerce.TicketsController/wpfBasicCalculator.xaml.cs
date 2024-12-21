using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfBasicCalculator : Window
    {
        private string Action { get; set; } = "";

        private double FirstNumber { get; set; } = 0;

		private double SecondNumber { get; set; } = 0;

        private double Result { get; set; } = 0;

		public string res = string.Empty;

		public wpfBasicCalculator()
		{
			if (Settings.Default.TopLeftOn)
			{
				this.Top = 50;
				this.Left = 50;
			}

			this.Topmost = true;

			InitializeComponent();
		}

		// EVENTS
		private void btn1_Click(object sender, EventArgs e) => AddTextToLabel(btn1);
		private void btn2_Click(object sender, EventArgs e) => AddTextToLabel(btn2);
		private void btn3_Click(object sender, EventArgs e) => AddTextToLabel(btn3);
		private void btn4_Click(object sender, EventArgs e) => AddTextToLabel(btn4);
		private void btn5_Click(object sender, EventArgs e) => AddTextToLabel(btn5);
		private void btn6_Click(object sender, EventArgs e) => AddTextToLabel(btn6);
		private void btn7_Click(object sender, EventArgs e) => AddTextToLabel(btn7);
		private void btn8_Click(object sender, EventArgs e) => AddTextToLabel(btn8);
		private void btn9_Click(object sender, EventArgs e) => AddTextToLabel(btn9);
		private void btn10_Click(object sender, EventArgs e) => AddTextToLabel(btn10);
		private void AddTextToLabel(Button button) => txtResult.Text += button.Content;

		private void btn11_Click(object sender, EventArgs e)
		{
			FirstNumber = Convert.ToDouble(txtResult.Text);
			txtResult.Text = "";
			Action = "+";
		}

		private void btn12_Click(object sender, EventArgs e)
		{
			FirstNumber = Convert.ToDouble(txtResult.Text);
			txtResult.Text = "";
			Action = "-";
		}

		private void btn13_Click(object sender, EventArgs e)
		{
			SecondNumber = Convert.ToDouble(txtResult.Text);

			if (Action == "+")
			{
				Result = FirstNumber + SecondNumber;
				txtResult.Text = Result.ToString();
			}
			if (Action == "-")
			{
				Result = FirstNumber - SecondNumber;
				txtResult.Text = Result.ToString();
			}
			if (Action == "*")
			{
				Result = FirstNumber * SecondNumber;
				txtResult.Text = Result.ToString();
			}
			if (Action == "/")
			{
                try
                {
					Result = FirstNumber / SecondNumber;
					txtResult.Text = Result.ToString();
				}
				catch (Exception ex)
                {
					MessageBox.Show("MATH ERROR: " + ex.Message, "Basic Calculator", MessageBoxButton.OK, MessageBoxImage.Error);
					txtResult.Text = "";
					FirstNumber = 0;
					SecondNumber = 0;
				}
			}
		}

		private void btn14_Click(object sender, EventArgs e)
		{
			txtResult.Text = "";
			FirstNumber = 0;
			SecondNumber = 0;
		}

		private void btn16_Click(object sender, EventArgs e)
		{
			FirstNumber = Convert.ToDouble(txtResult.Text);
			txtResult.Text = "";
			Action = "*";
		}

		private void btn17_Click(object sender, EventArgs e)
		{
			FirstNumber = Convert.ToDouble(txtResult.Text);
			txtResult.Text = "";
			Action = "/";
		}

		private void btn_Return(object sender, RoutedEventArgs e)
        {
			res = txtResult.Text;
			this.Close();
        }
    }
}
