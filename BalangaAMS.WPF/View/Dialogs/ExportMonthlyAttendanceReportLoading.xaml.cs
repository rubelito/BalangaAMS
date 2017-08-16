using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportMonthlyAttendanceReportLoading
    {
        private readonly MonthlyGroupInfotDTO _groupDto;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IMonthlyReport _monthlyReport;
        private readonly IBrethrenManager _brethrenManager;
        private readonly IGroupManager _groupManager;
        private readonly int _daysToConsiderNewlyBaptised;

        public ExportMonthlyAttendanceReportLoading(MonthlyGroupInfotDTO groupDto)
        {
            _groupDto = groupDto;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
            _monthlyReport = UnityBootstrapper.Container.Resolve<IMonthlyReport>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _daysToConsiderNewlyBaptised = Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
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
            var threadObject = (MonthlyGroupInfotDTO)e.Argument;
            var exportMonthlyReport = UnityBootstrapper.Container.Resolve<IExportMonthlyAttendanceReport>();
            DataTable reportTable;

            reportTable = GetReportTable(threadObject);
            if (IsReportTableIsNullOrEmpty(reportTable))
                throw new ArgumentException("Cannot Generate Report, there is no Brethren in this Group");
            var reportInfo = CreateAttendanceInfo(threadObject, reportTable);

            exportMonthlyReport.ExportMonthlyAttendanceReport(reportInfo);
        }

        private MonthlyAttendanceGroupInfo CreateAttendanceInfo(MonthlyGroupInfotDTO threadObject, DataTable reporTable)
        {
            var reportInfo = new MonthlyAttendanceGroupInfo(reporTable, threadObject.DestinationPath);
            reportInfo.GroupName = threadObject.Group.GroupName;
            reportInfo.MonthofYear = threadObject.MonthofYear;
            reportInfo.Year = threadObject.Year;
            reportInfo.DivisionName = threadObject.DivisionName;
            reportInfo.DistrictName = threadObject.DistrictName;
            return reportInfo;
        }

        private DataTable GetReportTable(MonthlyGroupInfotDTO threadObject)
        {
            DataTable reportTable;
            if (threadObject.Group.GroupName == "No Group")
                reportTable = GetReportTableForNoGroup(threadObject.MonthofYear, threadObject.Year);
            else if (threadObject.Group.GroupName == "Newly Baptised")
                reportTable = GetReporTableForNewlyBaptisedBrethren(threadObject.MonthofYear, threadObject.Year);
            else
                reportTable = GetReportOfGroupTable(threadObject.Group.Id, threadObject.MonthofYear, threadObject.Year);
            return reportTable;
        }


        private DataTable GetReportTableForNoGroup(MonthofYear monthofYear, int year)
        {
            var brethrenList = _groupManager.GetBrethrenWithNoGroup();
            var brethrenWithNoGroup = RemoveNewlyBaptisedInList(brethrenList);
            var reportTable = new DataTable();
            foreach (var brethren in brethrenWithNoGroup)
            {
                var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethren.Id, monthofYear, year);
                reportTable.Merge(brethrenReport);
            }
            return reportTable;
        }

        private DataTable GetReporTableForNewlyBaptisedBrethren(MonthofYear monthofYear, int year)
        {
            var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
            var newlyBapstised =
                brethrenWithNoGroup.Where(
                    b => _brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now))
                    .ToList();
            var reportTable = new DataTable();
            foreach (var brethren in newlyBapstised)
            {
                var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethren.Id, monthofYear, year);
                reportTable.Merge(brethrenReport);
            }
            return reportTable;
        }

            private List<BrethrenBasic> RemoveNewlyBaptisedInList(List<BrethrenBasic> brethrenList)
            {
                return
                    brethrenList.Where(b => !_brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now))
                        .ToList();
            }

            private DataTable GetReportOfGroupTable(long groupId, MonthofYear monthofYear, int year)
            {
                List<BrethrenBasic> brethrenList = _groupManager.GetBrethrenWithInGroup(groupId)
                    .Where(b => b.LocalStatus == LocalStatus.Present_Here)
                    .ToList();

                var reportTable = new DataTable();
                foreach (BrethrenBasic brethrenBasic in brethrenList)
                {
                    var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethrenBasic.Id, monthofYear, year);
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

        private bool IsReportTableIsNullOrEmpty(DataTable reportTable)
        {
            if (reportTable == null)
                return true;
            if (reportTable.Rows.Count == 0)
                return true;
            return false;
        }
    }
}
