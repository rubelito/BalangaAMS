using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using BalangaAMS.WPF.View.HelperClass;
using BalangaAMS.WPF.View.Report;
using BalangaAMS.WPF.View.Schedule;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ReportOption
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IBrethrenManager _brethrenManager;

        public ReportOption()
        {
            InitializeComponent();
            _sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            FillComboBoxWithYearMonth();
            FillSearchBoxWithBrethren();
        }

        private void FillComboBoxWithYearMonth()
        {
            var sessions = _sessionRetriever.FindGatheringSessions(g => g.IsStarted);
            var yearMonthGetter = new YearMonthGetter();
            var yearMonth = yearMonthGetter.GetYearMonth(sessions).OrderByDescending(y => y.Date);
            CboYearMonth.DataContext = yearMonth;
            CboIndividualYearMonth.DataContext = yearMonth;
        }

        private void FillSearchBoxWithBrethren(){
            IndividualMonthGroupBox.DataContext = _brethrenManager.GetAllBrethren();
        }

        private void AutoCompleteBoxName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var brethren = AutoCompleteBoxName.SelectedItem as BrethrenBasic;
            if (brethren != null){
                AutoCompleteBoxName.SearchText = string.Empty;
                AddBrethrenToListView(brethren);
            }
        }

        private void AutoCompleteBoxChurchId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var brethren = AutoCompleteBoxChurchId.SelectedItem as BrethrenBasic;
            if (brethren != null)
            {
                AutoCompleteBoxChurchId.SearchText = string.Empty;
                AddBrethrenToListView(brethren);
            }
        }

        private void AddBrethrenToListView(BrethrenBasic brethren)
        {
            if (!BrethrenListView.Items.Contains(brethren))
            BrethrenListView.Items.Add(brethren);
        }

        private void RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            var brethren = BrethrenListView.SelectedItem;
            BrethrenListView.Items.Remove(brethren);
        }

        private void MonthlyReportButton_Click(object sender, RoutedEventArgs e)
        {
            var monthYear = CboYearMonth.SelectedItem as DisplayMonthYearDTO;
            if (monthYear == null)
                return;
            var reportAuthorization = new ReportAuthorization();
           
            if (MonthlyRb.IsChecked == true)
            {
                var montlyreport = new MonthlyAttendanceReportForm(monthYear.Month, monthYear.Year, reportAuthorization);
                montlyreport.Owner = this;
                montlyreport.Show();
            }
            else
            {
                ProcessWeeklyReport(monthYear);
            }
        }

        private void ProcessWeeklyReport(DisplayMonthYearDTO monthYear)
        {
            var selectSchedule = new SelectSchedule(monthYear.Date);
            selectSchedule.SelectGatherings.Visibility = Visibility.Visible;
            selectSchedule.CanSelectNotStartedGathering = true;
            selectSchedule.Owner = this;
            selectSchedule.ShowDialog();
            if (!selectSchedule.IsCanceled())
            {
                var gatherings = selectSchedule.GetSelectedGatherings();
                var weeklyReport = new WeeklyAttendanceReportForm(gatherings);
                weeklyReport.Owner = this;
                weeklyReport.ShowDialog();
            } 
        }

        private void IdividualViewReport_Click(object sender, RoutedEventArgs e){
            var monthYear = CboIndividualYearMonth.SelectedItem as DisplayMonthYearDTO;
            if (monthYear == null)
                return;
            var brethren = GetBrethrenInListView();

            if (IndividualMonthlyRb.IsChecked == true){
                var monthlyreport = new IndividualMonthlyAttendanceReportForm(brethren, monthYear.Month, monthYear.Year);
                monthlyreport.Owner = this;
                monthlyreport.Show();
            }
            else{
                ProcessIndividualWeeklyReport(brethren, monthYear);
            }
        }

        private void ProcessIndividualWeeklyReport(List<BrethrenBasic> brethrenList, DisplayMonthYearDTO monthYear)
        {
            var selectSchedule = new SelectSchedule(monthYear.Date);
            selectSchedule.SelectGatherings.Visibility = Visibility.Visible;
            selectSchedule.CanSelectNotStartedGathering = true;
            selectSchedule.Owner = this;
            selectSchedule.ShowDialog();
            if (!selectSchedule.IsCanceled())
            {
                var gatherings = selectSchedule.GetSelectedGatherings();
                var individualWeeklyReport = new IndividualWeeklyAttendanceReportForm(gatherings, brethrenList);
                individualWeeklyReport.Owner = this;
                individualWeeklyReport.Show();
            }
        }

        private List<BrethrenBasic> GetBrethrenInListView()
        {
            var brethrenList = new List<BrethrenBasic>();
            foreach (BrethrenBasic brethren in BrethrenListView.Items)
            {
                brethrenList.Add(brethren);
            }
            return brethrenList;
        }

        private void ViewBdayCelebrants_Click(object sender, RoutedEventArgs e)
        {
            var bDayCelebrants = new ReportBirthDayCelebrants();
            bDayCelebrants.Owner = this;
            bDayCelebrants.Show();
        }

        private void ViewGroupingsButton_Click(object sender, RoutedEventArgs e)
        {
            var reportGroup = new ReportGroup();
            reportGroup.Owner = this;
            reportGroup.Show();
        }
    }
}
