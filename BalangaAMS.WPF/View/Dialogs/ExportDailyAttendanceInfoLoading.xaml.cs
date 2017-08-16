using System;
using System.ComponentModel;
using System.Windows;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportDailyAttendanceInfoLoading
    {
        private readonly DailyAttendanceDTO _dailyAttendanceDTO;
        private readonly BackgroundWorker _backgroundWorker;

        public ExportDailyAttendanceInfoLoading(DailyAttendanceDTO dailyAttendanceDTO)
        {
            _dailyAttendanceDTO = dailyAttendanceDTO;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
        }

        public void InitializeBackGroundWorker()
        {
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync(_dailyAttendanceDTO);           
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var threadObject = (DailyAttendanceDTO)e.Argument;
            var dailyExporter = UnityBootstrapper.Container.Resolve<IExportDailyAttendanceInfo>();

            dailyExporter.ExportDailyAttendanceInfo(threadObject.AttendanceInfoList, threadObject.SelectedSession,
                threadObject.Title, threadObject.FileName);
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
            Close();
        }
    }
}
