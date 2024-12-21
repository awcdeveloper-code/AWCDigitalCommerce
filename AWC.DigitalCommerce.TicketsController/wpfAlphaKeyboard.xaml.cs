using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfAlphaKeyboard : Window
    {
        private int type = 0;
        public string alphaKeyed = string.Empty;

        public wpfAlphaKeyboard(int _type)
        {
            type = _type;
            this.Topmost = true;
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            if (type > 0 && Settings.Default.UsePrefixPopup)
            {
                List<string> prefixList = DB.GetPrefixesByType(type);

                if (prefixList.Count > 0)
                {
                    this.Topmost = false;

                    this.Opacity = 0.5;
                    wpfFrequentItems freqPrefix = new wpfFrequentItems(prefixList);
                    freqPrefix.ShowDialog();
                    this.Opacity = 1;

                    if (freqPrefix.itemSelected)
                    {
                        alphaKeyed = freqPrefix.fip;
                        DB.InsertPrefix(type, alphaKeyed);
                        this.Close();
                    }
                }
            }
            this.Topmost = true;
            txtAlpha.Focus();
        }

        private void txtAlpha_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                alphaKeyed = txtAlpha.Text.ToUpper().Replace(";", "Ñ");

                if (type > 0)
                {
                    DB.InsertPrefix(type, alphaKeyed);
                }

                this.Close();
            }
        }

        #region BUTTONS 1 2 3 4 5 6 7 8 9 0
        private void btn_1(object sender, RoutedEventArgs e) { txtAlpha.Text += "1"; }
        private void btn_2(object sender, RoutedEventArgs e) { txtAlpha.Text += "2"; }
        private void btn_3(object sender, RoutedEventArgs e) { txtAlpha.Text += "3"; }
        private void btn_4(object sender, RoutedEventArgs e) { txtAlpha.Text += "4"; }
        private void btn_5(object sender, RoutedEventArgs e) { txtAlpha.Text += "5"; }
        private void btn_6(object sender, RoutedEventArgs e) { txtAlpha.Text += "6"; }
        private void btn_7(object sender, RoutedEventArgs e) { txtAlpha.Text += "7"; }
        private void btn_8(object sender, RoutedEventArgs e) { txtAlpha.Text += "8"; }
        private void btn_9(object sender, RoutedEventArgs e) { txtAlpha.Text += "9"; }
        private void btn_0(object sender, RoutedEventArgs e) { txtAlpha.Text += "0"; }
        #endregion

        #region BUTTONS Q W E R T Y U I O P
        private void btn_Q(object sender, RoutedEventArgs e) { txtAlpha.Text += "Q"; }
        private void btn_W(object sender, RoutedEventArgs e) { txtAlpha.Text += "W"; }
        private void btn_E(object sender, RoutedEventArgs e) { txtAlpha.Text += "E"; }
        private void btn_R(object sender, RoutedEventArgs e) { txtAlpha.Text += "R"; }
        private void btn_T(object sender, RoutedEventArgs e) { txtAlpha.Text += "T"; }
        private void btn_Y(object sender, RoutedEventArgs e) { txtAlpha.Text += "Y"; }
        private void btn_U(object sender, RoutedEventArgs e) { txtAlpha.Text += "U"; }
        private void btn_I(object sender, RoutedEventArgs e) { txtAlpha.Text += "I"; }
        private void btn_O(object sender, RoutedEventArgs e) { txtAlpha.Text += "O"; }
        private void btn_P(object sender, RoutedEventArgs e) { txtAlpha.Text += "P"; }
        #endregion

        #region BUTTONS A S D F G H J K L
        private void btn_A(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "A";
        }

        private void btn_S(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "S";
        }
        
        private void btn_D(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "D";
        }
        
        private void btn_F(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "F";
        }
        
        private void btn_G(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "G";
        }
        
        private void btn_H(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "H";
        }
        
        private void btn_J(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "J";
        }
        
        private void btn_K(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "K";
        }
        
        private void btn_L(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "L";
        }
        #endregion

        #region BUTTONS Z X C V B N M
        private void btn_Z(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "Z";
        }

        private void btn_X(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "X";
        }

        private void btn_C(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "C";
        }

        private void btn_V(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "V";
        }

        private void btn_B(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "B";
        }

        private void btn_N(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "N";
        }

        private void btn_M(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += "M";
        }
        #endregion

        #region BUTTON BACKSPACE + ENTER + SPACEBAR
        private void btn_Back(object sender, RoutedEventArgs e)
        {
            if (txtAlpha.Text.Length == 0) return;

            txtAlpha.Text = txtAlpha.Text.Substring(0, txtAlpha.Text.Length - 1);
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            alphaKeyed = txtAlpha.Text.ToUpper().Replace(";", "Ñ");

            if (type > 0)
            {
                DB.InsertPrefix(type, alphaKeyed);
            }

            this.Close();
        }

        private void btn_SPACE(object sender, RoutedEventArgs e)
        {
            txtAlpha.Text += " ";
        }
        #endregion
    }
}
