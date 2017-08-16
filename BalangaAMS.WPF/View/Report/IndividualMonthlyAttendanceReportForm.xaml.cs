using System.Collections.Generic;
using System.Data;
using System.Windows;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for IndividualMonthlyAttendanceReportForm.xaml
    /// </summary>
    public partial class IndividualMonthlyAttendanceReportForm
    {
        private readonly IMonthlyReportSummaryGetter _reportSummaryGetter;
        private readonly IMonthlyReport _monthlyReport;
        private readonly ISettingsManager _settingsManager;
        private readonly List<BrethrenBasic> _brethrenList;
        private readonly MonthofYear _monthofYear;
        private readonly int _year;

        public IndividualMonthlyAttendanceReportForm(List<BrethrenBasic> brethrenList, MonthofYear monthofYear, int year)
        {
            _brethrenList = brethrenList;
            _monthofYear = monthofYear;
            _year = year;
            _monthlyReport = UnityBootstrapper.Container.Resolve<IMonthlyReport>();
            _reportSummaryGetter = UnityBootstrapper.Container.Resolve<IMonthlyReportSummaryGetter>();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (_brethrenList == null || _brethrenList.Count == 0)
            {
                MessageBox.Show("Cannot View Report, No selected brethren");
                Close();
            }
            CreateReport();
        }

        private void CreateReport()
        {
            DataTable reportTable;
            reportTable = GetReportTable();
            var reportSummary = GetSummaryReport();
            var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
            instanceReportSource.ReportDocument = new IndividualMonthyAttendanceReport(reportTable, reportSummary,
                _settingsManager.GetLocalName());

            ReportViewer.ReportSource = instanceReportSource;
            ReportViewer.RefreshReport();
        }

        private DataTable GetReportTable()
        {
            var reportTable = new DataTable();
            foreach (BrethrenBasic b in _brethrenList)
            {
                var brethrenReport = _monthlyReport.GenerateBrethrenReport(b.Id, _monthofYear, _year);
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

        private IndividualReportSummary GetSummaryReport()
        {
            var summaryReport = _reportSummaryGetter.GetIndividualReportSummary(_monthofYear, _year);
            return summaryReport;
        }
    }
}
