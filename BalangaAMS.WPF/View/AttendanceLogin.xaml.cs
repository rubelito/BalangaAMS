using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.HelperClass;
using BalangaAMS.WPF.View.SupportClass;
using DPFP;
using DPFP.Capture;
using DPFP.Verification;
using Microsoft.Practices.Unity;
using Keyboard = System.Windows.Input.Keyboard;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for AttendanceLogin.xaml
    /// </summary>
    public partial class AttendanceLogin : DPFP.Capture.EventHandler
    {
        private readonly List<GatheringSession> _sessions;
        private readonly IImageService _imageService;
        private readonly IAttendanceLogger _attendanceLogger;
        private readonly IAttendanceRetriever _attendanceRetriever;
        private readonly IChurchIdManager _churchIdManager;
        private readonly IOtherLocalManager _otherLocalManager;
        private readonly List<BrethrenBasic> _brethrenList;
        private readonly List<ChurchId> _churchIds; 
        private readonly List<BrethrenBasic> _brethrenWithFingerPrint;
        private Capture _capturer;
        private Verification _verificator;
        private Dictionary<long, Template> _fingerPrintCache;
        private readonly Timer _timer;
        private bool _isLate;
        private int _attendeesCount;
        private int _otherLocalCount;

        public AttendanceLogin(List<GatheringSession> sessions){
            _sessions = sessions;
            _imageService = UnityBootstrapper.Container.Resolve<IImageService>();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            
            _attendanceLogger = UnityBootstrapper.Container.Resolve<IAttendanceLogger>();
            _attendanceRetriever = UnityBootstrapper.Container.Resolve<IAttendanceRetriever>();
            _churchIdManager = UnityBootstrapper.Container.Resolve<IChurchIdManager>();
            _otherLocalManager = UnityBootstrapper.Container.Resolve<IOtherLocalManager>();
            
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;

            _brethrenList = brethrenManager.FindBrethren(b => b.LocalStatus == LocalStatus.Present_Here);
            _brethrenWithFingerPrint = _brethrenList.Where(b => b.FingerPrint != null).ToList();
            _churchIds = _churchIdManager.GetAllChurchIds();

            LoadFingerPrintCache();

            InitializeComponent();

            FillSearchBoxWithBrethren();
            ImageControl.Source = ImageToBitmap.ConvertToBitmapImage(_imageService.GetDefaultPicture());

        }

        private void LoadFingerPrintCache(){
            _fingerPrintCache = new Dictionary<long, Template>();

            foreach (var brethren in _brethrenWithFingerPrint)
                _fingerPrintCache.Add(brethren.Id, new Template(new MemoryStream(brethren.FingerPrint.Template)));
        }

        private void AttendanceLogin_Loaded_1(object sender, RoutedEventArgs e){
            InitializeRichTextBox();
            DisplayGatheringInfo();
            _timer.Start();
            InitiateDevice();
            StartCapture();
        }

        private void InitializeRichTextBox(){
            var paragraph = new Paragraph();
            var flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(paragraph);
            Logs.Document = flowDocument;
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e){
            StopCapture();
        }

        private void DisplayGatheringInfo(){
            StringBuilder strBuilder = new StringBuilder();
            foreach (var session in _sessions){
                strBuilder.Append(session.Gatherings + " / ");
            }
            GatheringTextBlock.Text = strBuilder.ToString();
            GatheringDateTextBlock.Text = _sessions[0].Date.ToString("MMM dd, yyyy");
        }

        private void FillSearchBoxWithBrethren(){
            SearchGroupBox.DataContext = _brethrenList;
            SearchOtherLocal.DataContext = _churchIds;
        }

        private void AutoCompleteBoxName_SelectionChanged(object sender, SelectionChangedEventArgs e){
            var brethren = AutoCompleteBoxName.SelectedItem as BrethrenBasic;
            if (brethren != null){
                AutoCompleteBoxName.SearchText = string.Empty;                
                DisplayBrethrenInfo(brethren);
                LogAttendanceToDatabase(brethren);
            }
        }

        private void AutoCompleteBoxChurchId_SelectionChanged(object sender, SelectionChangedEventArgs e){
            var brethren = AutoCompleteBoxChurchId.SelectedItem as BrethrenBasic;
            if (brethren != null){
                AutoCompleteBoxChurchId.SearchText = string.Empty;                
                DisplayBrethrenInfo(brethren);
                LogAttendanceToDatabase(brethren);
            }
        }

        private void AutoCompleteBoxOtherLocal_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter){
                var id = AutoCompleteBoxOtherLocal.SelectedItem as ChurchId;
                if (id != null){
                    AutoCompleteBoxOtherLocal.SearchText = string.Empty;
                    DisplayOtherLocal(id.Code);
                    LogInOtherLocalAttendance(id.Code);                   
                }
                else{
                    string newChurdId = AutoCompleteBoxOtherLocal.SearchText.ToUpper();
                    if (!string.IsNullOrWhiteSpace(newChurdId)){
                        AutoCompleteBoxOtherLocal.SearchText = string.Empty;
                        CreateNewEntryOfOtherLocal(newChurdId);
                        DisplayOtherLocal(newChurdId);
                        LogInOtherLocalAttendance(newChurdId);
                    }
                }
            }
        }

        private void AutoCompleteBoxOtherLocal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var id = AutoCompleteBoxOtherLocal.SelectedItem as ChurchId;
            if (id != null){
                AutoCompleteBoxOtherLocal.SearchText = string.Empty;
                DisplayOtherLocal(id.Code);
                LogInOtherLocalAttendance(id.Code);
            }
        }

        private void CreateNewEntryOfOtherLocal(string churchId){
            var newId = new ChurchId();
            newId.Code = churchId;
            _churchIdManager.AddChurchId(newId);
        }

        private void LogInOtherLocalAttendance(string churchId){
            foreach (var s in _sessions){
                var l = new OtherLocalLog();
                l.ChurchId = churchId;
                l.IsLate = _isLate;
                l.DateTime = DateTime.Now;
                _otherLocalManager.LogAttendance(l, s.Id);
            }
        }

        private void LogAttendanceToDatabase(BrethrenBasic brethren){
            foreach (var session in _sessions){
                var attendanceLog = CreateAttendanceLog(brethren);
                session.AttendanceLogs.Add(attendanceLog);
                _attendanceLogger.Logbrethren(session.Id, attendanceLog);
            }
        }

        private AttendanceLog CreateAttendanceLog(BrethrenBasic brethren){
            var attendanceLog = new AttendanceLog{
                BrethrenId = brethren.Id,
                DateTime = DateTime.Now,
                IsLate = _isLate
            };
            return attendanceLog;
        }

        private void DisplayBrethrenInfo(BrethrenBasic brethren){
            var bitmapImage = ImageToBitmap.ConvertToBitmapImage(_imageService.GetPicture(brethren.Id));
            bitmapImage.Freeze();
            Dispatcher.Invoke(new Function(delegate{
                ChurchIdBlock.Text = !string.IsNullOrWhiteSpace(brethren.ChurchId) ? brethren.ChurchId : "No Church ID";
                NameBlock.Text = brethren.Name;
                ImageControl.Source = bitmapImage;
                if (IsAlreadyLogin(brethren)){
                    DisplayMessage(brethren.Name + " is already Login", Brushes.IndianRed);
                    return;
                }
                if (_isLate)
                    DisplayMessage("Welcome: " + brethren.Name + " - Late", Brushes.DarkGoldenrod);
                else
                    DisplayMessage("Welcome: " + brethren.Name, Brushes.DarkGreen);
                _attendeesCount++;

                LoginCountText.Text = Convert.ToString(_attendeesCount);
            }));
        }

        private void DisplayOtherLocal(string churchId){
            var bitmapImage = ImageToBitmap.ConvertToBitmapImage(_imageService.GetDefaultPicture());
            bitmapImage.Freeze();
            Dispatcher.Invoke(new Function(delegate{
                ChurchIdBlock.Text = churchId;
                NameBlock.Text = string.Empty;
                ImageControl.Source = bitmapImage;

                bool isAlreadyLoggedIn = false;

                foreach (var s in _sessions){
                    if (_otherLocalManager.IsAlreadyLogin(churchId, s.Id)){
                        isAlreadyLoggedIn = true;
                        break;
                    }
                }

                if (isAlreadyLoggedIn)
                {
                    DisplayMessage(churchId + " is already Login", Brushes.IndianRed);
                    return;
                }
                if (_isLate)
                    DisplayMessage("Welcome other local: " + churchId + " - Late", Brushes.DarkGoldenrod);
                else
                    DisplayMessage("Welcome other local: " + churchId, Brushes.DarkGreen);

                _otherLocalCount++;
                OtherLocalCountText.Text = Convert.ToString(_otherLocalCount);
            }));
        }

        private bool IsAlreadyLogin(BrethrenBasic brethren){
            return _sessions[0].AttendanceLogs.Any(a => a.BrethrenId == brethren.Id) ||
                   _attendanceRetriever.IsAlreadyLogin(brethren.Id, _sessions[0]);
        }

        private void InitiateDevice(){
            try{
                _capturer = new Capture();
                _verificator = new Verification();
                if (null != _capturer)
                    _capturer.EventHandler = this;
                else
                    DisplayReaderStatus("Error 13: Can't initiate capture operation!");
            }
            catch{
                DisplayReaderStatus("Error 12:");
            }
        }

        private void StartCapture(){
            if (null != _capturer){
                try{
                    _capturer.StartCapture();
                    DisplayReaderStatus("Place you finger to the reader");
                }
                catch{
                    DisplayReaderStatus("Error 11: Can't initiate capture!, Please Check if the device is Ready");
                }
            }
        }

        private void StopCapture(){
            _capturer.StopCapture();
            _capturer.Dispose();
        }

        public void OnComplete(object capture, string readerSerialNumber, Sample sample){
            ProcessFingerPrint(sample);
        }

        private void ProcessFingerPrint(Sample sample){
            DisplayFingerPrint(sample);
            VerifyFingerPrint(sample);
        }

        private void DisplayFingerPrint(Sample sample){
            var bitmap = DigitalPersonaSupportClass.ConvertToBitmap(sample);
            var bitmapImage = DigitalPersonaSupportClass.ConverToBitmapImage(bitmap);
            DisplayToImageControl(bitmapImage);
        }

        private void VerifyFingerPrint(Sample sample){
            var features = DigitalPersonaSupportClass.ExtractFeatureSet(sample, DPFP.Processing.DataPurpose.Verification);
            if (IsQualityOk(features)){
                var churchIdList = GetMatchingFingerPrint(features);
                if (IsResultsAreEmptyOrMoreThanOne(churchIdList)){
                    DisplayMessage("Unknown FingerPrint, Please Scan again", Brushes.DarkRed);
                }
                else{
                    var brethren = _brethrenWithFingerPrint.Find(b => b.Id == churchIdList.FirstOrDefault());                    
                    DisplayBrethrenInfo(brethren);
                    LogAttendanceToDatabase(brethren);
                }
            }
            else
                DisplayMessage("FingerPrint Quality is not good, Please Scan again", Brushes.DarkRed);
        }

        private bool IsQualityOk(FeatureSet features){
            return features != null;
        }

        private bool IsResultsAreEmptyOrMoreThanOne(List<long> results){
            return results.Count == 0 || results.Count > 1;
        }

        private List<long> GetMatchingFingerPrint(FeatureSet features){
            var idList = new List<long>();

            Parallel.ForEach(_fingerPrintCache, p => {
                var result = CompareFeaturesWithTemplateAndReturnResult(features, p.Value);
                if (result.Verified){
                    idList.Add(p.Key);
                }
            });
            return idList;
        }

        private Verification.Result CompareFeaturesWithTemplateAndReturnResult(FeatureSet features, Template template){
            var result = new Verification.Result();
            _verificator.Verify(features, template, ref result);
            return result;
        }

        public void OnFingerGone(object capture, string readerSerialNumber){

        }

        public void OnFingerTouch(object capture, string readerSerialNumber){

        }

        public void OnReaderConnect(object capture, string readerSerialNumber){
            DisplayReaderStatus("FingerPrint Reader Connected");
            DisplayMessage("FingerPrint Reader Connected", Brushes.Black);
            DisplayMessage("Place you finger to the reader", Brushes.Black);
        }

        public void OnReaderDisconnect(object capture, string readerSerialNumber){
            DisplayReaderStatus("FingerPrint Reader Disconnected");
        }

        public void OnSampleQuality(object capture, string readerSerialNumber, CaptureFeedback captureFeedback){

        }

        private void DisplayMessage(string message, Brush brushes){
            Dispatcher.Invoke(new Function(delegate{
                var run = new Run(message + "\u2028"){
                    FontSize = 22,
                    Foreground = brushes,
                    FontWeight = FontWeights.UltraBold
                };

                ((Paragraph) Logs.Document.Blocks.FirstBlock).Inlines.Add(run);
                Logs.ScrollToEnd();
            }));
        }

        private void DisplayReaderStatus(string message){
            Dispatcher.Invoke(new Function(delegate{
                DeviceStatus.Text = message;
            }));
        }

        private void DisplayToImageControl(BitmapImage bitmapImage){
            bitmapImage.Freeze();
            Dispatcher.Invoke(new Function(delegate{
                FingerPrintImage.Source = bitmapImage;
            }));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e){
            if (IsCtrlAndAltAndLIsPressed())
                ActivateLateMarker();
        }

        private void ActivateLateMarker(){
            if (_isLate == false){
                _isLate = true;
                TimeGroup.Background = Brushes.DarkGoldenrod;
            }

            else{
                _isLate = false;
                TimeGroup.Background = Brushes.LightBlue;
            }
        }

        private bool IsCtrlAndAltAndLIsPressed(){
            return (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt)) && Keyboard.IsKeyDown(Key.L);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e){
            Dispatcher.Invoke(new Function(delegate {
                TimeText.Text = DateTime.Now.ToString("hh:mm:ss tt");
            }));
        }
    }
}
