using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportIndividualMonthlyAttendanceReportLoading
    {
        private readonly IndividualMonthlyInfoDTO _groupDto;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IMonthlyReport _monthlyReport;

        public ExportIndividualMonthlyAttendanceReportLoading(IndividualMonthlyInfoDTO groupDto)
        {
            _groupDto = groupDto;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
            _monthlyReport = UnityBootstrapper.Container.Resolve<IMonthlyReport>();
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
            var threadObject = (IndividualMonthlyInfoDTO)e.Argument;
            var exportMonthlyReport = UnityBootstrapper.Container.Resolve<IExportMonthlyAttendanceReport>();
            DataTable reportTable;

            reportTable = GetReportTable(threadObject);
            var reportInfo = CreateAttendanceInfo(threadObject, reportTable);

            exportMonthlyReport.ExportMonthlyAttendanceReport(reportInfo);
        }

        private MonthlyAttendanceGroupInfo CreateAttendanceInfo(IndividualMonthlyInfoDTO threadObject, DataTable reporTable)
        {
            var reportInfo = new MonthlyAttendanceGroupInfo(reporTable, threadObject.DestinationPath);
            reportInfo.GroupName = "INDIVIDUAL";
            reportInfo.MonthofYear = threadObject.MonthofYear;
            reportInfo.Year = threadObject.Year;
            reportInfo.DivisionName = threadObject.DivisionName;
            reportInfo.DistrictName = threadObject.DistrictName;
            return reportInfo;
        }

        private DataTable GetReportTable(IndividualMonthlyInfoDTO threadObject)
        {
            var reportTable = new DataTable();
            foreach (var brethren in threadObject.Brethrens)
            {
                var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethren.Id, threadObject.MonthofYear,
                    threadObject.Year);
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
