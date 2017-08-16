using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using MessageBox = System.Windows.Forms.MessageBox;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for MonthlyAttendanceReportForm.xaml
    /// </summary>
    public partial class MonthlyAttendanceReportForm
    {
        private readonly MonthofYear _monthofYear;
        private readonly int _year;
        private readonly ReportAuthorization _reportAuthotization;
        private readonly IMonthlyReport _monthlyReport;
        private readonly IGroupManager _groupManager;
        private readonly IBrethrenManager _brethrenManager;
        private readonly IMonthlyReportSummaryGetter _iSummaryGetter;
        private readonly ISettingsManager _settingsManager;
        private readonly int _daysToConsiderNewlyBaptised;
        private List<Group> _groups;

        public MonthlyAttendanceReportForm(MonthofYear monthofYear, int year, ReportAuthorization reportAuthotization)
        {
            _monthofYear = monthofYear;
            _year = year;
            _reportAuthotization = reportAuthotization;

            InitializeComponent();
           
            _monthlyReport = UnityBootstrapper.Container.Resolve<IMonthlyReport>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _iSummaryGetter = UnityBootstrapper.Container.Resolve<IMonthlyReportSummaryGetter>();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
            _daysToConsiderNewlyBaptised = Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            FillComboBoxWithGroupNames();
        }

        private void FillComboBoxWithGroupNames()
        {
            _groups = _groupManager.Getallgroup();
            _groups.Add(new Group {GroupName = "Newly Baptised"});
            _groups.Add(new Group {GroupName = "No Group"});
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
                MessageBox.Show("Cannot generate report, no brethren in this group", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    reportTable = GetReportTable(group.Id);
                var reportSummary = ProcessReportSummary(group);
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
                        var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethren.Id, _monthofYear, _year);
                        var newBrethrenReport = ConvertTableToTelerikTable(brethrenReport);
                        reportTable.Merge(newBrethrenReport);
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
                        var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethren.Id, _monthofYear, _year);
                        var newBrethrenReport = ConvertTableToTelerikTable(brethrenReport);
                        reportTable.Merge(newBrethrenReport);
                    }
                    return reportTable;
                }

                private DataTable GetReportTable(long groupId)
                {
                    List<BrethrenBasic> brethrenList = _groupManager.GetBrethrenWithInGroup(groupId)
                        .Where(b => b.LocalStatus == LocalStatus.Present_Here)
                        .ToList();

                    var reportTable = new DataTable();
                    foreach (BrethrenBasic brethrenBasic in brethrenList)
                    {
                        var brethrenReport = _monthlyReport.GenerateBrethrenReport(brethrenBasic.Id, _monthofYear, _year);
                        var newBrethrenReport = ConvertTableToTelerikTable(brethrenReport);
                        reportTable.Merge(newBrethrenReport);
                    }
                    return reportTable;
                }

                private DataTable ConvertTableToTelerikTable(DataTable brethrenReport)
                {
                    var tableConverter = new ReportTableConverter();
                    var newTable = tableConverter.ConvertEnumTableReportToImageTable(brethrenReport);
                    return newTable;
                }

                private MontlyReportSummary ProcessReportSummary(Group group)
                {
                    var summaryReport = _iSummaryGetter.GetSummaryReport(group, _monthofYear, _year);
                    return summaryReport;
                }

                private void CreateTelerikReport(DataTable reportTable, MontlyReportSummary reportSummary)
                {
                    var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                    instanceReportSource.ReportDocument = new MonthlyAttendanceReport(reportTable, reportSummary, _settingsManager.GetLocalName());

                    ReportViewer1.ReportSource = instanceReportSource;
                    ReportViewer1.RefreshReport();
                }
    }
}
