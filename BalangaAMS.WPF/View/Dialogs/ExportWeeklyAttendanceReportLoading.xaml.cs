using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportWeeklyAttendanceReportLoading
    {
        private readonly WeeklyGroupInfoDTO _groupDto;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IWeeklyReport _weeklyReport;
        private readonly IBrethrenManager _brethrenManager;
        private readonly IGroupManager _groupManager;
        private readonly int _daysToConsiderNewlyBaptised;
        private List<GatheringSession> _sessions; 

        public ExportWeeklyAttendanceReportLoading(WeeklyGroupInfoDTO groupDto)
        {
            _groupDto = groupDto;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
            _weeklyReport = UnityBootstrapper.Container.Resolve<IWeeklyReport>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _daysToConsiderNewlyBaptised = Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
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
            var threadObject = (WeeklyGroupInfoDTO)e.Argument;
            var exportWeeklyReport = UnityBootstrapper.Container.Resolve<IExportWeeklyAttendanceReport>();
            DataTable reportTable;

            reportTable = GetReportTable(threadObject);
            var reportInfo = CreateAttendanceInfo(threadObject, reportTable);

            exportWeeklyReport.ExportWeeklyAttendanceReport(reportInfo);
        }

        private WeeklyAttendanceGroupInfo CreateAttendanceInfo(WeeklyGroupInfoDTO threadObject, DataTable reporTable)
        {
            var reportInfo = new WeeklyAttendanceGroupInfo(reporTable, threadObject.DestinationPath);
            reportInfo.GroupName = threadObject.Group.GroupName;
            reportInfo.DateCoverage = GatheringsDateArranger.GetDateCoverage(_sessions);
            reportInfo.DivisionName = threadObject.DivisionName;
            reportInfo.DistrictName = threadObject.DistrictName;
            return reportInfo;
        }
        
        private DataTable GetReportTable(WeeklyGroupInfoDTO threadObject)
        {
            DataTable reportTable;
            if (threadObject.Group.GroupName == "No Group")
                reportTable = GetReportTableForNoGroup();
            else if (threadObject.Group.GroupName == "Newly Baptised")
                reportTable = GetReporTableForNewlyBaptisedBrethren();
            else
                reportTable = GetReportTable(threadObject.Group);
            return reportTable;
        }

        private DataTable GetReporTableForNewlyBaptisedBrethren()
        {
            var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
            var newlyBapstised = brethrenWithNoGroup.Where(
                    b => _brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now))
                    .ToList();
            var reportTable = new DataTable();
            foreach (var brethren in newlyBapstised)
            {
                var brethrenReport = _weeklyReport.GetBrethrenReport(brethren.Id, _sessions);
                reportTable.Merge(brethrenReport);
            }
            return reportTable;
        }

        private DataTable GetReportTableForNoGroup()
        {
            var brethrenList = _groupManager.GetBrethrenWithNoGroup();
            var brethrenWithNoGroup = RemoveNewlyBaptisedInList(brethrenList);
            var reportTable = new DataTable();
            foreach (var brethren in brethrenWithNoGroup)
            {
                var brethrenReport = _weeklyReport.GetBrethrenReport(brethren.Id, _sessions);
                reportTable.Merge(brethrenReport);
            }
            return reportTable;
        }

        private DataTable GetReportTable(Group group)
        {
            List<BrethrenBasic> brethrenList = _groupManager.GetBrethrenWithInGroup(group.Id)
                .Where(b => b.LocalStatus == LocalStatus.Present_Here)
                .ToList();

            var reportTable = new DataTable();
            foreach (BrethrenBasic brethren in brethrenList)
            {
                var brethrenReport = _weeklyReport.GetBrethrenReport(brethren.Id, _sessions);
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

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null){
                throw new Exception(e.Error.Message);
            }
            Close();
        }
    }
}
