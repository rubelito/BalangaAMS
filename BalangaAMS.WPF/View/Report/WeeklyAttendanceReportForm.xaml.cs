using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using MessageBox = System.Windows.MessageBox;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for WeeklyAttendanceReportForm.xaml
    /// </summary>
    public partial class WeeklyAttendanceReportForm
    {
        private IWeeklyReport _weeklyReport;
        private readonly List<GatheringSession> _sessions;
        private List<Group> _groups;
        private readonly IGroupManager _groupManager;
        private readonly IBrethrenManager _brethrenManager;
        private readonly int _daysToConsiderNewlyBaptised;
        private readonly ISettingsManager _settingsManager;

        public WeeklyAttendanceReportForm(List<GatheringSession> sessions)
        {
            _sessions = sessions;
            InitializeComponent();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
            _weeklyReport = UnityBootstrapper.Container.Resolve<IWeeklyReport>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _daysToConsiderNewlyBaptised = Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            FillComboBoxWithGroupNames();
        }

        private void FillComboBoxWithGroupNames()
        {
            _groups = _groupManager.Getallgroup();
            _groups.Add(new Group{GroupName = "Newly Baptised"});
            _groups.Add(new Group{GroupName = "No Group"});
            CboGroup.DataContext = _groups;
        }

        private void CboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var group = CboGroup.SelectedItem as Group;
            if (group == null)
                return;
            if (IsGroupHasBrethren(group))
                CreateReport(group);
            else
                MessageBox.Show("Cannot generate report, no brethren in this group", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

            private bool IsGroupHasBrethren(Group group)
            {
                bool hasBrethren = false;
                if (IsNewlyBaptisedGroup(group))
                {
                    var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
                    hasBrethren = IsNewlyBaptisedGroupHasBrethren(brethrenWithNoGroup);
                }
                else if (IsNoGroup(group))
                {
                    var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
                    hasBrethren = HasBrethren(brethrenWithNoGroup);
                }
                else
                {
                    var brethren = _groupManager.GetBrethrenWithInGroup(group.Id);
                    if (brethren.Count > 0)
                    {
                        hasBrethren = true;
                    }
                }
                return hasBrethren;
            }

                private bool IsNewlyBaptisedGroup(Group group)
                {
                    return group.GroupName == "Newly Baptised";
                }

                private bool IsNoGroup(Group group)
                {
                    return group.GroupName == "No Group";
                }

                private bool IsNewlyBaptisedGroupHasBrethren(List<BrethrenBasic> brethrenList)
                {
                    return brethrenList.Any(b => b.Group == null && 
                        _brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now));
                }

                private bool HasBrethren(List<BrethrenBasic> brethrenList)
                {
                    var brethrenWithNoGroup = RemoveNewlyBaptisedInList(brethrenList);
                    return brethrenWithNoGroup.Count > 0;
                }

                    private List<BrethrenBasic> RemoveNewlyBaptisedInList(List<BrethrenBasic> brethrenList)
                    {
                        return
                            brethrenList.Where(b => !_brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now))
                                .ToList();
                    }

        private void CreateReport(Group group)
        {
            DataTable reportTable;
            if (IsNewlyBaptisedGroup(group))
                reportTable = GetReporTableForNewlyBaptisedBrethren();
            else if (IsNoGroup(group))
                reportTable = GetReportTableForNoGroup();
            else
                reportTable = GetReportTable(group);

            var reportSummary = GetReportSummary(group);
            CreateTelerikReport(reportTable, reportSummary);            
        }    

            private DataTable GetReporTableForNewlyBaptisedBrethren()
            {
                var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
                var newlyBapstised =
                    brethrenWithNoGroup.Where(
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

            private WeeklyReportSummary GetReportSummary(Group group)
            {
                var dateCoverage = GatheringsDateArranger.GetDateCoverage(_sessions);
                var reportSummary = new WeeklyReportSummary() {GroupName = group.GroupName, DateCoverage = dateCoverage};
                return reportSummary;
            }

            private void CreateTelerikReport(DataTable reportTable, WeeklyReportSummary reportSummary)
            {
                var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                instanceReportSource.ReportDocument = new WeeklyAttendanceReport(reportTable, reportSummary, _settingsManager.GetLocalName());

                ReportViewer1.ReportSource = instanceReportSource;
                ReportViewer1.RefreshReport();
            }
    }
}
