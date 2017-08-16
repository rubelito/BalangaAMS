using System.Collections.Generic;
using System.Data;
using System.Windows;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for WeeklyAttendanceReportForm.xaml
    /// </summary>
    public partial class IndividualWeeklyAttendanceReportForm
    {
        private IWeeklyReport _weeklyReport;
        private readonly ISettingsManager _settingsManager;
        private readonly List<GatheringSession> _sessions;
        private readonly List<BrethrenBasic> _brethrenList;

        public IndividualWeeklyAttendanceReportForm(List<GatheringSession> sessions, List<BrethrenBasic> brethrenList)
        {
            _sessions = sessions;
            _brethrenList = brethrenList;
            InitializeComponent();
            _weeklyReport = UnityBootstrapper.Container.Resolve<IWeeklyReport>();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            CreateReport();
        }

        private void CreateReport()
        {
            var reportTable = GetReportTable();
            var dateCoverage = GatheringsDateArranger.GetDateCoverage(_sessions);
            var weeklySummary = new WeeklyReportSummary {GroupName = "INDIVIDUAL", DateCoverage = dateCoverage};
            var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
            instanceReportSource.ReportDocument = new WeeklyAttendanceReport(reportTable, weeklySummary,
                _settingsManager.GetLocalName());

            ReportViewer1.ReportSource = instanceReportSource;
            ReportViewer1.RefreshReport();
        }

        private DataTable GetReportTable()
        {
            var reportTable = new DataTable();
            foreach (BrethrenBasic brethren in _brethrenList)
            {
                var brethrenReport = _weeklyReport.GetBrethrenReport(brethren.Id, _sessions);
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
    }
}
