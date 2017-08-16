using System;
using System.ComponentModel;
using System.Windows;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.Core.Domain;
using System.Collections.Generic;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.ApplicationLayer.Interfaces;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportFinalAttendanceReport
    {
        private readonly FinalAttendanceReport _report;
        private readonly BackgroundWorker _backgroundWorker;

        public ExportFinalAttendanceReport(FinalAttendanceReport report)
        {
            _report = report;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
        }

        private void InitializeBackGroundWorker()
        {
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync(_report);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var threadObject = (FinalAttendanceReport) e.Argument;
            var sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var attendeesRetriever = UnityBootstrapper.Container.Resolve<IAttendeesRetriever>();

            List<GatheringSession> gatherings =
                sessionRetriever.GetAllStartedRegularGatheringsForMonthOf(threadObject.Month, threadObject.Year);
            threadObject.Gatherings = attendeesRetriever.GetAttendees(gatherings);
            var exporter = new FinalMonthlyAttendanceReportExporter();
            exporter.Export(threadObject);
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
