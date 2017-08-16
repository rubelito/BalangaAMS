using System.Windows;
using System.Windows.Media.Imaging;
using BalangaAMS.Core.Domain;
using BalangaAMS.WPF.View.SupportClass;
using DPFP;
using DPFP.Error;
using DPFP.Processing;

namespace BalangaAMS.WPF.View
{
    delegate void Function();

    public partial class EnrollForm : DPFP.Capture.EventHandler
    {
        private readonly BrethrenBasic _brethren;
        private DPFP.Capture.Capture _capturer;
        private Enrollment _enroller;
        private bool _isCanceled;

        public EnrollForm(BrethrenBasic brethren){
            _brethren = brethren;
            InitializeComponent();
        }

        private void EnrollForm_Loaded_1(object sender, RoutedEventArgs e){
            FillBrethrenInfo();
            Enroll.IsEnabled = false;
            InitiateDevice();
            StartCapture();
        }

        private void FillBrethrenInfo(){
            ChurchIdBlock.Text = _brethren.ChurchId;
            NameBlock.Text = _brethren.Name;
        }

        private void InitiateDevice(){
            try{
                _capturer = new DPFP.Capture.Capture();
                if (null != _capturer)
                    _capturer.EventHandler = this;
                else
                    DisplayReaderStatus("Can't initiate capture operation!");
            }
            catch{
                MessageBox.Show("Can't initiate capture operation!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void StartCapture(){
            if (null != _capturer){
                try{
                    _capturer.StartCapture();
                    _enroller = new Enrollment();
                    DisplayReaderStatus("Place you finger to the reader");
                }
                catch{
                    DisplayReaderStatus("Can't initiate capture!");
                }
            }
        }

        private void StopCapture(){
            _capturer.StopCapture();
            _capturer.Dispose();
        }

        private void ExtractAndAddFeature(Sample sample){
            var featureSet = DigitalPersonaSupportClass.ExtractFeatureSet(sample, DataPurpose.Enrollment);
            if (featureSet == null){
                MessageBox.Show("FingerPrint Sample not good enough, try again", "Note", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                ClearImageAndEnroller();
                return;
            }
            AddToTemplate(featureSet);
            DisplayFingerPrint(sample);
        }

        private void AddToTemplate(FeatureSet featureSet){
            _enroller.AddFeatures(featureSet);
            if (_enroller.TemplateStatus == Enrollment.Status.Ready)
                Dispatcher.Invoke(new Function(delegate {
                    Enroll.IsEnabled = true;
                }));
        }

        private void DisplayFingerPrint(Sample sample){
            var bitmap = DigitalPersonaSupportClass.ConvertToBitmap(sample);
            var bitmapImage = DigitalPersonaSupportClass.ConverToBitmapImage(bitmap);
            DisplayToImageControl(bitmapImage);
        }

        private void DisplayToImageControl(BitmapImage bitmapImage){
            bitmapImage.Freeze();
            Dispatcher.Invoke(new Function(delegate{
                switch (_enroller.FeaturesNeeded){
                    case 3:
                        Image1.Source = bitmapImage;
                        break;
                    case 2:
                        Image2.Source = bitmapImage;
                        break;
                    case 1:
                        Image3.Source = bitmapImage;
                        break;
                    case 0:
                        Image4.Source = bitmapImage;
                        break;
                }
            }));
        }

        public void OnComplete(object capture, string readerSerialNumber, Sample sample){
            try{
                StartProcessingFingerPrint(sample);
            }
            catch (SDKException ex){
                if (ex.ErrorCode == ErrorCodes.InvalidFeatureSet){
                    MessageBox.Show("Cannot Generate FingerPrint Template, You must use the same Finger for Enrolling",
                        "Cannot Enroll", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    ClearImageAndEnroller();
                }
            }
        }

        private void StartProcessingFingerPrint(Sample sample){
            if (_enroller == null)
                _enroller = new Enrollment();

            if (_enroller.TemplateStatus != Enrollment.Status.Ready)
                ExtractAndAddFeature(sample);
        }

        public void OnFingerGone(object capture, string readerSerialNumber){

        }

        public void OnFingerTouch(object capture, string readerSerialNumber){

        }

        public void OnReaderConnect(object capture, string readerSerialNumber){
            DisplayReaderStatus("FingerPrint Reader Connected");
        }

        public void OnReaderDisconnect(object capture, string readerSerialNumber){
            DisplayReaderStatus("FingerPrint Reader Disconnected");
        }

        public void OnSampleQuality(object capture, string readerSerialNumber,
            DPFP.Capture.CaptureFeedback captureFeedback){

        }

        private void DisplayReaderStatus(string status){
            Dispatcher.Invoke(new Function(delegate {
                DeviceStatus.Text = status;
            }));
        }

        private void Clear_Click(object sender, RoutedEventArgs e){
            ClearImageAndEnroller();
        }

        private void ClearImageAndEnroller(){
            Dispatcher.Invoke(new Function(delegate{
                Image1.Source = null;
                Image2.Source = null;
                Image3.Source = null;
                Image4.Source = null;
                Enroll.IsEnabled = false;
            }));

            _enroller = null;
        }

        private void Enroll_Click(object sender, RoutedEventArgs e){
            EnrollFingerPrint();
            _isCanceled = false;
            Close();
        }

        private void EnrollFingerPrint(){
            Serialize();
            MessageBox.Show("FingerPrint Enrolled", "Succesful Enrolling", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Serialize(){
            _brethren.FingerPrint = new BrethrenFingerPrint{
                Template = _enroller.Template.Bytes
            };
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e){
            StopCapture();
        }

        private void Close_Click(object sender, RoutedEventArgs e){
            _isCanceled = true;
            Close();
        }

        public bool IsCanceled(){
            return _isCanceled;
        }
    }
}
