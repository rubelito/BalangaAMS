using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.WPF.View.EnumData;
using BalangaAMS.WPF.View.HelperClass;
using BalangaAMS.WPF.View.Schedule;
using Microsoft.Practices.Unity;
using Image = System.Drawing.Image;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main
    {
        private readonly string _imageDirectory;

        public Main(){
            UnityBootstrapper.Configure();
            InitializeComponent();
            _imageDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                              ConfigurationManager.AppSettings["systemimagedirectory"];
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e){
            AddImageToButtons();
            AddTitlesAndLogoName("Unknown");
            InitiateUserLogin();
        }

        private void InitiateUserLogin(){
            var userLogin = new UserLogin();
            userLogin.Owner = this;
            userLogin.ShowDialog();
            if (!userLogin.IsExited()){
                if (userLogin.GetUserType() == UserType.Admin)
                    AddTitlesAndLogoName("Administrator");
                else if (userLogin.GetUserType() == UserType.Member)
                    DisableMemberControl();
                else if (userLogin.GetUserType() == UserType.None){
                    MessageBox.Show("Unkown User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else{
                Close();
            }
        }

        private void DisableMemberControl(){
            AddTitlesAndLogoName("Member");
            SettingsButton.IsEnabled = false;
            ExportButton.IsEnabled = false;
            ExcelReportButton.IsEnabled = false;
            EnrollButton.IsEnabled = false;
            ReportButton.IsEnabled = false;
            ManageGroupingsButton.IsEnabled = false;
            ManageScheduleButton.IsEnabled = false;
            ManageMasterListButton.IsEnabled = false;
        }   

        private void AddImageToButtons(){
            Logging.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "logging.png"));
            ManualLogging.Source =
                ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "manuallogging.png"));
            ManageSchedule.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "schedule.png"));
            ManageMasterList.Source =
                ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "masterlist.png"));
            ManageGroupings.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "group.png"));
            ReportImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "reports.png"));
            ExportImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "export.png"));
            EnrollImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "enroll.png"));
            ExcelReportImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "excel.png"));
            SettingsImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "settings.png"));
            AttendanceRemoveImage.Source = ImageToBitmap.ConvertToBitmapImage(Image.FromFile(_imageDirectory + "removecheck.png"));
        }

        private void AddTitlesAndLogoName(string user){
            var settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
            Title = settingsManager.GetLocalName() + " AMS - " + user;
            AMSName.Text = settingsManager.GetLocalName() + " ATTENDANCE MONITORING SYSTEM";
        }

        private void LoggingButton_Click(object sender, RoutedEventArgs e){
            try{
                var selectSchedule = new SelectSchedule(DateTime.Now)
                {
                    SelectGatherings = { Visibility = Visibility.Visible },
                    CanSelectNotStartedGathering = false,
                    Owner = this
                };
                selectSchedule.ShowDialog();
                if (!selectSchedule.IsCanceled())
                {
                    var selectedGatherings = selectSchedule.GetSelectedGatherings();
                    var attendanceLogin = new AttendanceLogin(selectedGatherings);
                    attendanceLogin.Owner = this;
                    attendanceLogin.Show();
                }
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message + " -" + ex.InnerException.Message, ex.GetType().FullName);
            }
            
        }

        private void ManualLoggingButton_Click(object sender, RoutedEventArgs e){
            var manualLogging = new ManualLogging();
            manualLogging.Owner = this;
            manualLogging.ShowDialog();
        }

        private void ManageScheduleButton_Click(object sender, RoutedEventArgs e){
            var manageGatherings = new ManageGatherings(DateTime.Now);
            manageGatherings.Owner = this;
            manageGatherings.ShowDialog();
        }

        private void ManageMasterListButton_Click(object sender, RoutedEventArgs e){
            try{
                var masterlist = new ManageBrethren();
                masterlist.Owner = this;
                masterlist.ShowDialog();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }            
        }

        private void ManageGroupingsButton_Click(object sender, RoutedEventArgs e){
            var manageGroupings = new ManageGroupings();
            manageGroupings.Owner = this;
            manageGroupings.ShowDialog();
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e){
            try{
                var report = new ReportOption();
                report.Owner = this;
                report.ShowDialog();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message + " - " + ex.InnerException.Message, ex.GetType().FullName);
            }            
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e){
            var export = new ExportData();
            export.Owner = this;
            export.ShowDialog();
        }

        private void EnrollButton_Click(object sender, RoutedEventArgs e){
            try{
                var fingerPrintEnrollment = new FingerPrintEnrollment();
                fingerPrintEnrollment.Owner = this;
                fingerPrintEnrollment.ShowDialog();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }            
        }

        private void ExcelReportButton_Click(object sender, RoutedEventArgs e){
            var excelReportOption = new ExcelReportOption();
            excelReportOption.Owner = this;
            excelReportOption.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e){
            var settingsForm = new SettingsForm();
            settingsForm.Owner = this;
            settingsForm.ShowDialog();
        }

        private void AttendanceRemoveButton_Click(object sender, RoutedEventArgs e){
            var removeAttendance = new AttendanceRemover();
            removeAttendance.Owner = this;
            removeAttendance.ShowDialog();
        }
    }
}
