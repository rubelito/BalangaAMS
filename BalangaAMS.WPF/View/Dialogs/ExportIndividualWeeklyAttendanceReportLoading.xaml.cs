using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportIndividualWeeklyAttendanceReportLoading
    {
        private readonly IndividualWeeklyInfoDTO _groupDto;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IWeeklyReport _weeklyReport;
        private List<GatheringSession> _sessions; 

        public ExportIndividualWeeklyAttendanceReportLoading(IndividualWeeklyInfoDTO groupDto)
        {
            _groupDto = groupDto;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
            _weeklyReport = UnityBootstrapper.Container.Resolve<IWeeklyReport>();
            _sessions = groupDto.GatheringSessions;
        }

        public void InitializeBackGroundWorker()
        {
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync(_groupDto);           
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var threadObject = (IndividualWeeklyInfoDTO)e.Argument;
            var exportWeeklyReport = UnityBootstrapper.Container.Resolve<IExportWeeklyAttendanceReport>();
            DataTable reportTable;

            reportTable = GetReportTable(threadObject);
            var reportInfo = CreateAttendanceInfo(threadObject, reportTable);

            exportWeeklyReport.ExportWeeklyAttendanceReport(reportInfo);
        }

        private WeeklyAttendanceGroupInfo CreateAttendanceInfo(IndividualWeeklyInfoDTO threadObject, DataTable reporTable)
        {
            var reportInfo = new WeeklyAttendanceGroupInfo(reporTable, threadObject.DestinationPath);
            reportInfo.GroupName = "INDIVIDUAL";
            reportInfo.DateCoverage = GatheringsDateArranger.GetDateCoverage(_sessions);
            reportInfo.DivisionName = threadObject.DivisionName;
            reportInfo.DistrictName = threadObject.DistrictName;
            return reportInfo;
        }

        private DataTable GetReportTable(IndividualWeeklyInfoDTO threadObject)
        {
            var reportTable = new DataTable();
            foreach (var brethren in threadObject.Brethrens)
            {
                var brethrenReport = _weeklyReport.GetBrethrenReport(brethren.Id, threadObject.GatheringSessions);
                reportTable.Merge(brethrenReport);
            }
            return reportTable;
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
