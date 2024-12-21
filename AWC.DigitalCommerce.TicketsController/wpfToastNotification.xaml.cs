using System;
using System.Timers;
using System.Windows;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfToastNotification : Window
    {
        private Timer timer;
        
        public wpfToastNotification(string msg, int duration)
        {
            InitializeComponent();

            BusinessName.Text = Settings.Default.BusinessName;
            ToastMessage.Text = msg;

            timer = new Timer(duration);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            Dispatcher.Invoke(() => Close());
        }
    }
}
