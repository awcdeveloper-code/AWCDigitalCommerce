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
    /// Interaction logic for wpfMessageBox.xaml
    /// </summary>
    public partial class wpfMessageBox : Window
    {
        private string lang = string.Empty;
        public enum MessageBoxType
        {
            ConfirmationWithYesNo = 0,
            ConfirmationWithYesNoCancel,
            Information,
            Error,
            Warning
        }

        public enum MessageBoxImage
        {
            Warning = 0,
            Question,
            Information,
            Error,
            None
        }
        public wpfMessageBox()
        {
            InitializeComponent();
        }

        static wpfMessageBox _messageBox;
        static MessageBoxResult _result = MessageBoxResult.Yes;
        
        public static MessageBoxResult Show (string caption, string msg, MessageBoxType type, string lang)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(caption, msg, MessageBoxButton.YesNo, MessageBoxImage.Question, lang);
                case MessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(caption, msg, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, lang);
                case MessageBoxType.Information:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Information, lang);
                case MessageBoxType.Error:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Error, lang);
                case MessageBoxType.Warning:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Warning, lang);
                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageBoxType type, string lang)
        {
            return Show(string.Empty, msg, type, lang);
        }

        public static MessageBoxResult Show(string msg, string lang)
        {
            return Show(string.Empty, msg,MessageBoxButton.OK, MessageBoxImage.None, lang);
        }

        public static MessageBoxResult Show (string caption, string text, string lang)
        {
            return Show(caption, text, MessageBoxButton.OK, MessageBoxImage.None, lang);
        }

        public static MessageBoxResult Show (string caption, string text, MessageBoxButton button, string lang)
        {
            return Show(caption, text, button, MessageBoxImage.None, lang);
        }

        public static MessageBoxResult Show (string caption, string text, MessageBoxButton button, MessageBoxImage image, string lang)
        {
            _messageBox = new wpfMessageBox { txtMsg = { Text = text }, MessageTitle = { Text = caption } };
            Traductor.ApplyTranslation(_messageBox, lang);
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            _messageBox.ShowDialog();
            return _result;
        }

        private static void SetVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnOk.Focus();
                    break;
                case MessageBoxButton.OKCancel:
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                default:
                    break;
            }
        }

        private static void SetImageOfMessageBox(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    _messageBox.SetImage("Warning.png");
                    break;
                case MessageBoxImage.Question:
                    _messageBox.SetImage("Question.png");
                    break;
                case MessageBoxImage.Information:
                    _messageBox.SetImage("Information.png");
                    break;
                case MessageBoxImage.Error:
                    _messageBox.SetImage("Error.png");
                    break;
                default:
                    _messageBox.img.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                _result = MessageBoxResult.OK;
            else if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            else if (sender == btnCancel)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;
            _messageBox.Close();
            _messageBox = null;
        }
        
        private void SetImage(string imageName)
        {
            string uri = string.Format("/Images/{0}", imageName);
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            img.Source = new BitmapImage(uriSource);
        }
    }
}
