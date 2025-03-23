using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using DPFP.Verification;
using DPFP.Gui;
using DPFP.Error;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfFingerprintScanner : Window, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        private DPFP.Processing.Enrollment Enroller;
        private DPFP.Template newTemplate;

        public wpfFingerprintScanner()
        {
            InitializeComponent();
        }

        private void Init()
        {
            try
            {
                Capturer = new DPFP.Capture.Capture();

                if (Capturer != null)
                    Capturer.EventHandler = this;

                Enroller = new DPFP.Processing.Enrollment();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing the fingerprint reader: " + ex.Message);
            }
        }

        private void btnStartCapture_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
            btnStartCapture.IsEnabled = false;
            btnStopCapture.IsEnabled = true;
        }

        private void btnStopCapture_Click(object sender, RoutedEventArgs e)
        {
            StopCapture();
            btnStartCapture.IsEnabled = true;
            btnStopCapture.IsEnabled = false;
        }

        private void StartCapture()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StartCapture();
                    txtStatus.Text = "Status: Fingerprint reader is ready.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to start capture: " + ex.Message);
                }
            }
        }

        private void StopCapture()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                    txtStatus.Text = "Status: Capture stopped.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to stop capture: " + ex.Message);
                }
            }
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            Dispatcher.Invoke(() =>
            {
                ProcessFingerprint(Sample);
                txtStatus.Text = "Status: Fingerprint captured!";
            });
        }

        private void ProcessFingerprint(DPFP.Sample Sample)
        {
            try
            {
                DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
                DPFP.FeatureSet features = new DPFP.FeatureSet();
                CaptureFeedback feedback = CaptureFeedback.None;

                extractor.CreateFeatureSet(Sample, DPFP.Processing.DataPurpose.Enrollment, ref feedback, ref features);

                if (feedback == CaptureFeedback.Good)
                {
                    Enroller.AddFeatures(features);
                    txtStatus.Text = "Status: Fingerprint sample added.";

                    // Convert sample to image and display in WPF Image control
                    imgFingerprint.Source = ConvertSampleToBitmap(Sample);

                    if (Enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
                    {
                        MemoryStream stream = new MemoryStream();
                        Enroller.Template.Serialize(stream);
                        newTemplate = new DPFP.Template(stream);
                        txtStatus.Text = "Status: Fingerprint enrolled successfully!";
                        StopCapture();
                    }
                }
                else
                {
                    txtStatus.Text = "Status: Poor quality fingerprint. Try again.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing fingerprint: " + ex.Message);
            }
        }

        private BitmapSource ConvertSampleToBitmap(DPFP.Sample sample)
        {
            DPFP.Capture.SampleConversion converter = new DPFP.Capture.SampleConversion();
            System.Drawing.Bitmap bitmap = null;
            converter.ConvertToPicture(sample, ref bitmap);
            if (bitmap == null) return null;

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCapture();
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback Feedback) { }
    }
}
