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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfRequestPIN.xaml
    /// </summary>
    public partial class wpfRequestPIN : Window
    {
        private bool firstNum = true;
        public string numKeyed = string.Empty;

        public wpfRequestPIN()
        {
            this.Topmost = true;

            InitializeComponent();

            this.KeyDown += new KeyEventHandler(wpfRequestPIN_KeyUp);

            PINNum.Focus();
        }

        private void wpfRequestPIN_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (PINNum.Password.Length > 0)
                        numKeyed = PINNum.Password;
                    else
                        numKeyed = "0";
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Request PIN was closed.", Logger.Severity.INFORMATION);
                    this.Close();
                    break;
            }
        }
        #region KEYBOARD
        private void btn_Num7(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password= string.Empty;
                firstNum = false;
            }
            PINNum.Password += "7";
            PINNum.Focus();
        }

        private void btn_Num8(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "8";
        }

        private void btn_Num9(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "9";
        }

        private void btn_Num4(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "4";
        }

        private void btn_Num5(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "5";
        }

        private void btn_Num6(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "6";
        }

        private void btn_Num1(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "1";
        }

        private void btn_Num2(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "2";
        }

        private void btn_Num3(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "3";
        }

        private void btn_Clean(object sender, RoutedEventArgs e)
        {
            PINNum.Password = string.Empty;
            PINNum.Focus();
        }

        private void btn_Num0(object sender, RoutedEventArgs e)
        {
            if (firstNum)
            {
                PINNum.Password = string.Empty;
                firstNum = false;
            }
            PINNum.Password += "0";
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (PINNum.Password.Length > 0)
                numKeyed = PINNum.Password;
            else
                numKeyed = "0";

            this.Close();
        }

        #endregion
    }
}
