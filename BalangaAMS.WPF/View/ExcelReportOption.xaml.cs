using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.Dialogs;
using BalangaAMS.WPF.View.DTO;
using BalangaAMS.WPF.View.EnumData;
using BalangaAMS.WPF.View.HelperClass;
using BalangaAMS.WPF.View.Schedule;
using Microsoft.Practices.Unity;
using MessageBox = System.Windows.Forms.MessageBox;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for ExcelReportOption.xaml
    /// </summary>
    public partial class ExcelReportOption
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IBrethrenManager _brethrenManager;
        private readonly IGroupManager _groupManger;
        private readonly IAttendanceRetriever _attendanceRetriever;
        private readonly SaveFileDialog _saveFileDialog;
        private readonly string _divisonName;
        private readonly string _districtName;

        public ExcelReportOption(){
            InitializeComponent();
            _divisonName = ConfigurationManager.AppSettings["DivisionName"];
            _districtName = ConfigurationManager.AppSettings["DistrictName"];
            _sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _groupManger = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _attendanceRetriever = UnityBootstrapper.Container.Resolve<IAttendanceRetriever>();
            _saveFileDialog = new SaveFileDialog();
            PrepareSaveDialog();
        }

        private void PrepareSaveDialog(){
            _saveFileDialog.Filter = "Excel Documents (*.xlsx)|*.xlsx";
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e){
            FillComboBoxWithYearMonth();
            FillComboBoxWithGroupings();
            FillSearchBoxWithBrethren();
        }

        private void FillComboBoxWithYearMonth(){
            var sessions = _sessionRetriever.FindGatheringSessions(g => g.IsStarted);
            var yearMonthGetter = new YearMonthGetter();
            var yearMonth = yearMonthGetter.GetYearMonth(sessions).OrderByDescending(y => y.Date);
            CboYearMonth.DataContext = yearMonth;
            CboYearMonthForAll.DataContext = yearMonth;
            CboIndividualYearMonth.DataContext = yearMonth;
            CboYearMonthSpecific.DataContext = yearMonth;
        }

        private void FillComboBoxWithGroupings(){
            var groups = _groupManger.Getallgroup();
            groups.AddRange(AddNoGroupAndNewlyBaptisedGroup());
            CboGroup.DataContext = groups;
        }

        private List<Group> AddNoGroupAndNewlyBaptisedGroup(){
            var groups = new List<Group>{
                new Group{GroupName = "No Group"},
                new Group{GroupName = "Newly Baptised"}
            };
            return groups;
        }

        private void FillSearchBoxWithBrethren(){
            IndividualMonthGroupBox.DataContext = _brethrenManager.GetAllBrethren();
        }

        private void AutoCompleteBoxName_SelectionChanged(object sender, SelectionChangedEventArgs e){
            var brethren = AutoCompleteBoxName.SelectedItem as BrethrenBasic;
            if (brethren != null){
                AutoCompleteBoxName.SearchText = string.Empty;
                AddBrethrenToListView(brethren);
            }
        }

        private void AutoCompleteBoxChurchId_SelectionChanged(object sender, SelectionChangedEventArgs e){
            var brethren = AutoCompleteBoxChurchId.SelectedItem as BrethrenBasic;
            if (brethren != null){
                AutoCompleteBoxChurchId.SearchText = string.Empty;
                AddBrethrenToListView(brethren);
            }
        }

        private void AddBrethrenToListView(BrethrenBasic brethren){
            if (!BrethrenListView.Items.Contains(brethren))
                BrethrenListView.Items.Add(brethren);
        }

        private void RemoveFromList_Click(object sender, RoutedEventArgs e){
            var brethren = BrethrenListView.SelectedItem;
            BrethrenListView.Items.Remove(brethren);
        }

        private void MonthlyReportButton_Click(object sender, RoutedEventArgs e){
            var monthYear = CboYearMonth.SelectedItem as DisplayMonthYearDTO;
            var group = CboGroup.SelectedItem as Group;
            if (monthYear == null || group == null) return;

            if (MonthlyRb.IsChecked == true){
                string fileName = CreateMonthlyFileName(group.GroupName, monthYear.YearMonth);
                PrepareExportingMonthlyReport(group, monthYear, fileName);
            }
            else{
                PrepareExportingWeeklyGroupReport(monthYear, group);
            }
        }

        private void PrepareExportingMonthlyReport(Group group, DisplayMonthYearDTO monthYear, string fileName){
            var dialogResult = OpenSaveDialogBox(fileName);
            if (IsDialogBoxIsCanceled(dialogResult)) return;
            var groupReportDTO = InitiazeGroupMonthlyInfo(group, _saveFileDialog.FileName, monthYear);
            ProcessMonthlyReport(groupReportDTO);
        }

        private void PrepareExportingWeeklyGroupReport(DisplayMonthYearDTO monthYear, Group group){
            var sessions = GetSelectedGatherings(monthYear);
            if (sessions.Count == 0) return;
            string fileName = group.GroupName + ", " + GatheringsDateArranger.GetDateCoverage(sessions);
            var dialogResult = OpenSaveDialogBox(fileName);
            if (IsDialogBoxIsCanceled(dialogResult)) return;
            var groupReportDTO = InitializeGroupWeeklyInfo(_saveFileDialog.FileName, sessions, group);
            ProcessWeeklyReport(groupReportDTO);
        }

        private MonthlyGroupInfotDTO InitiazeGroupMonthlyInfo(Group group, string destinationPath,
            DisplayMonthYearDTO monthYear){
            var groupReportDtO = new MonthlyGroupInfotDTO{
                Group = group,
                DestinationPath = destinationPath,
                DistrictName = _districtName,
                DivisionName = _divisonName,
                MonthofYear = monthYear.Month,
                Year = monthYear.Year
            };
            return groupReportDtO;
        }

        private void ProcessMonthlyReport(MonthlyGroupInfotDTO groupReportDtO){
            var exportBrethrenDialog = new ExportMonthlyAttendanceReportLoading(groupReportDtO);
            try{
                exportBrethrenDialog.ShowDialog();
            }
            catch (Exception ex){
                exportBrethrenDialog.Close();
                MessageBox.Show(ex.Message, "Can't Export Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private List<GatheringSession> GetSelectedGatherings(DisplayMonthYearDTO monthYear){
            var selectedSessions = new List<GatheringSession>();
            var selectSchedule = new SelectSchedule(monthYear.Date);
            selectSchedule.SelectGatherings.Visibility = Visibility.Visible;
            selectSchedule.CanSelectNotStartedGathering = true;
            selectSchedule.ShowDialog();
            if (!selectSchedule.IsCanceled())
                selectedSessions = selectSchedule.GetSelectedGatherings();
            return selectedSessions;
        }

        private WeeklyGroupInfoDTO InitializeGroupWeeklyInfo(string destinationPath, List<GatheringSession> sessions,
            Group group){
            var groupReportDtO = new WeeklyGroupInfoDTO{
                DestinationPath = destinationPath,
                DivisionName = _divisonName,
                DistrictName = _districtName,
                GatheringSessions = sessions,
                Group = group
            };
            return groupReportDtO;
        }

        private void ProcessWeeklyReport(WeeklyGroupInfoDTO groupReportDtO){
            var exportBrethrenDialog = new ExportWeeklyAttendanceReportLoading(groupReportDtO);
            try{
                exportBrethrenDialog.ShowDialog();
            }
            catch (Exception ex){
                exportBrethrenDialog.Close();
                MessageBox.Show(ex.Message, "Can't Export Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void IdividualExportReport_Click(object sender, RoutedEventArgs e){
            var monthYear = CboIndividualYearMonth.SelectedItem as DisplayMonthYearDTO;
            if (monthYear == null)
                return;
            var brethren = GetBrethrenInListView();

            if (IndividualMonthlyRb.IsChecked == true){
                PrepareExportingIndividualMonthlyReport(monthYear, brethren);
            }
            else{
                PrepareExportingIndividualWeeklyReport(monthYear, brethren);
            }
        }

        private void PrepareExportingIndividualMonthlyReport(DisplayMonthYearDTO monthYear, List<BrethrenBasic> brethren){
            string fileName = CreateMonthlyFileName("Individual, ", monthYear.YearMonth);
            var dialogResult = OpenSaveDialogBox(fileName);
            if (IsDialogBoxIsCanceled(dialogResult)) return;
            var individualReportDTO = InitializeIndividualMonthlyInfo(brethren, _saveFileDialog.FileName, monthYear);
            ProcessIndividualMonthlyReport(individualReportDTO);
        }

        private void PrepareExportingIndividualWeeklyReport(DisplayMonthYearDTO monthYear, List<BrethrenBasic> brethrens){
            var sessions = GetSelectedGatherings(monthYear);
            if (sessions.Count == 0) return;
            string fileName = "Individual, " + GatheringsDateArranger.GetDateCoverage(sessions);
            var dialogResult = OpenSaveDialogBox(fileName);
            if (IsDialogBoxIsCanceled(dialogResult)) return;
            var individualReportDTO = InitializeIndividualWeeklyInfo(brethrens, sessions, _saveFileDialog.FileName);
            ProcessIndividualWeeklyReport(individualReportDTO);
        }

        private IndividualWeeklyInfoDTO InitializeIndividualWeeklyInfo(List<BrethrenBasic> brethrens,
            List<GatheringSession> sessions, string destinationPath){
            var individualReportDTO = new IndividualWeeklyInfoDTO{
                Brethrens = brethrens,
                GatheringSessions = sessions,
                DestinationPath = destinationPath,
                DivisionName = _divisonName,
                DistrictName = _districtName
            };
            return individualReportDTO;
        }

        private IndividualMonthlyInfoDTO InitializeIndividualMonthlyInfo(List<BrethrenBasic> brethrens,
            string destinationPath, DisplayMonthYearDTO monthYear){
            var individualReportDto = new IndividualMonthlyInfoDTO{
                Brethrens = brethrens,
                DestinationPath = destinationPath,
                DivisionName = _divisonName,
                DistrictName = _districtName,
                MonthofYear = monthYear.Month,
                Year = monthYear.Year
            };
            return individualReportDto;
        }

        private void ProcessIndividualMonthlyReport(IndividualMonthlyInfoDTO individualReportDTO){
            var exportBrethrenDialog = new ExportIndividualMonthlyAttendanceReportLoading(individualReportDTO);
            try{
                exportBrethrenDialog.ShowDialog();
            }
            catch (Exception ex){
                exportBrethrenDialog.Close();
                MessageBox.Show(ex.Message, "Can't Export Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void ProcessIndividualWeeklyReport(IndividualWeeklyInfoDTO individualReportDTO){
            var exportBrethrenDialog = new ExportIndividualWeeklyAttendanceReportLoading(individualReportDTO);
            try{
                exportBrethrenDialog.ShowDialog();
            }
            catch (Exception ex){
                exportBrethrenDialog.Close();
                MessageBox.Show(ex.Message, "Can't Export Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private List<BrethrenBasic> GetBrethrenInListView(){
            var brethrenList = new List<BrethrenBasic>();
            foreach (BrethrenBasic brethren in BrethrenListView.Items){
                brethrenList.Add(brethren);
            }
            return brethrenList.OrderBy(b => b.Name).ToList();
        }

        private string CreateMonthlyFileName(string groupName, string monthyYear){
            return groupName + ", " + monthyYear;
        }

        private void SpecificGatheringButton_Click(object sender, RoutedEventArgs e){
            var monthYear = CboYearMonthSpecific.SelectedItem as DisplayMonthYearDTO;
            var attendedOption = (GatheringTimeOption) CboGatheringTimeOption.SelectedValue;

            var sessions = GetSelectedGatherings(monthYear);
            if (sessions.Count == 0) return;
            if (IsSelectedMultipleGatherings(sessions)){
                MessageBox.Show("You must select only one gathering", "Can't Export Report", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var selectedSession = sessions.FirstOrDefault();
            ExportAttendanceInfo(attendedOption, selectedSession);
        }

        private void ExportAttendanceInfo(GatheringTimeOption attendedOption, GatheringSession selectedSession){
            string title;
            var dailyInfoList = GetSelectedOption(attendedOption, selectedSession, out title);

            var dialogResult =
                OpenSaveDialogBox(title + " - " + selectedSession.Gatherings +
                                  selectedSession.Date.ToString("(MMM dd, yyyy)"));
            if (IsDialogBoxIsCanceled(dialogResult))
                return;

            var dailyDto = CreateDailyAttendanceDTO(dailyInfoList, selectedSession, title,
                _saveFileDialog.FileName);

            try{
                var dailyAttendanceExporter = new ExportDailyAttendanceInfoLoading(dailyDto);
                dailyAttendanceExporter.ShowDialog();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Cannot Export Attendance Info");
            }

        }

        private DailyAttendanceDTO CreateDailyAttendanceDTO(List<AttendanceInfoDTO> attendanceInfoDtos,
            GatheringSession selectedSession, string title, string fileName){
            var dailyDto = new DailyAttendanceDTO{
                AttendanceInfoList = attendanceInfoDtos,
                SelectedSession = selectedSession,
                Title = title,
                FileName = fileName
            };
            return dailyDto;
        }

        private DialogResult OpenSaveDialogBox(string fileName){
            _saveFileDialog.FileName = fileName;
            return _saveFileDialog.ShowDialog();
        }

        private static bool IsDialogBoxIsCanceled(DialogResult dialogResult){
            return dialogResult != System.Windows.Forms.DialogResult.OK;
        }

        private List<AttendanceInfoDTO> GetSelectedOption(GatheringTimeOption attendedOption,
            GatheringSession selectedSession,
            out string title){
            List<AttendanceInfoDTO> dailyInfoList;

            switch (attendedOption){
                case GatheringTimeOption.Absent_Only:
                    dailyInfoList = _attendanceRetriever.GetAttendanceInfoOfBrethrenWhoIsAbsentInThisGathering(
                        selectedSession);
                    title = "Mga Kapatid na hindi dumalo sa pagkakatipon na ito.";
                    break;
                case GatheringTimeOption.Late_Only:
                    dailyInfoList = _attendanceRetriever.GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLate(
                        selectedSession).OrderBy(b => b.Brethren.Name).ToList();;

                    //Add Other Local
                    dailyInfoList.AddRange(
                        _attendanceRetriever.GetAttendanceInfoOfOtherLocalWhoAttendedThisGatheringLate(selectedSession));

                    title = "Mga Kapatid na late sa pagkakatipon na ito.";
                    return dailyInfoList;
                case GatheringTimeOption.Attended_Live:
                    dailyInfoList = _attendanceRetriever.GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLive(
                        selectedSession);
                    title = "Mga Kapatid na live dumalo sa pagkakatipon na ito.";
                    break;
                case GatheringTimeOption.Attended_Other_Day:
                    dailyInfoList = _attendanceRetriever.GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringNotLive(
                        selectedSession);
                    title = "Mga Kapatid na hindi dumalo ng Live sa pagkakatipon na ito.";
                    break;
                default:
                    dailyInfoList = _attendanceRetriever.GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(
                        selectedSession).OrderBy(b => b.Brethren.Name).ToList();

                    //Add Other Local
                    dailyInfoList.AddRange(
                        _attendanceRetriever.GetAttendanceInfoOfOtherLocalWhoAttendedThisGathering(selectedSession));
   
                    title = "Mga kapatid na dumalo sa pagkakatipon na ito.";
                    return dailyInfoList;
            }
            return dailyInfoList.OrderBy(b => b.Brethren.Name).ToList();
        }

        private bool IsSelectedMultipleGatherings(List<GatheringSession> sessions){
            return sessions.Count > 1;
        }

        private void MonthlyReportForAllButton_Click(object sender, RoutedEventArgs e){
            var monthYear = CboYearMonthForAll.SelectedItem as DisplayMonthYearDTO;
            if (monthYear == null) return;

            string fileName = monthYear.Month + " " + monthYear.Year + " Attendance Report";
            PrepareExportingAllBrethrenAttendanceReport(monthYear, fileName);
        }

        private void PrepareExportingAllBrethrenAttendanceReport(DisplayMonthYearDTO monthYear, string fileName){
            var dialogResult = OpenSaveDialogBox(fileName);
            if (IsDialogBoxIsCanceled(dialogResult)) return;

            var finalReportDto = InitializeFinalAttendanceReportDto(monthYear, _saveFileDialog.FileName);           

            try{
                var exportForm = new ExportFinalAttendanceReport(finalReportDto);
                exportForm.ShowDialog();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Cannot Export Attendance.");
            }
        }

        private FinalAttendanceReport InitializeFinalAttendanceReportDto(DisplayMonthYearDTO monthYear, string fileName){
            var reportDto = new FinalAttendanceReport();
            reportDto.Month = monthYear.Month;
            reportDto.Year = monthYear.Year;
            reportDto.DestinationPath = fileName;

            return reportDto;
        }
    }
}
