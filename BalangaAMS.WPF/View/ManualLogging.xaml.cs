using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using BalangaAMS.WPF.View.Schedule;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for ManualLogging.xaml
    /// </summary>
    public partial class ManualLogging
    {       
        private readonly IAttendanceLogger _attendanceLogger;
        private readonly IAttendanceRetriever _attendanceRetriever;
        private ObservableCollection<BrethrenBasic> _brethrenCollection;
        private readonly ObservableCollection<BrethrenListViewCheckDTO> _brethrenInfoList;
        private List<GatheringSession> _gatherings;

        public ManualLogging(){
            InitializeComponent();
            _attendanceLogger = UnityBootstrapper.Container.Resolve<IAttendanceLogger>();
            _attendanceRetriever = UnityBootstrapper.Container.Resolve<IAttendanceRetriever>();
            _brethrenInfoList = new ObservableCollection<BrethrenListViewCheckDTO>();
            BrethrenListView.DataContext = _brethrenInfoList;
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

        private void SelectGatheringButton_Click(object sender, RoutedEventArgs e){
            var selectGathering = new SelectSchedule(DateTime.Now);
            selectGathering.CanSelectNotStartedGathering = false;
            selectGathering.ShowDialog();
            if (!selectGathering.IsCanceled()){
                _gatherings = selectGathering.GetSelectedGatherings();
                SetGatheringBoxInfo(_gatherings);
                var notAttendedBrethren = _attendanceRetriever.GetBrethrenWhoIsAbsentToThisGathering(_gatherings[0]);
                LoadBrethrenToAutoCompletebox(notAttendedBrethren);
                _brethrenInfoList.Clear();
            }
        }

        private void LoadBrethrenToAutoCompletebox(List<BrethrenBasic> brethrenList){
            _brethrenCollection =
                new ObservableCollection<BrethrenBasic>(brethrenList);
            SearchGroupBox.DataContext = CollectionViewSource.GetDefaultView(_brethrenCollection);
        }

        private void SetGatheringBoxInfo(List<GatheringSession> gatherings){
            if (IsSelectedGatheringIsMoreThanOne(gatherings)){
                CreateCombinedGathering(gatherings);
            }
            else{
                GatheringsName.Text = gatherings[0].Gatherings.ToString();
                GatheringDate.Text = gatherings[0].Date.ToString("MMM dd, yyyy");
            }
        }

        private bool IsSelectedGatheringIsMoreThanOne(List<GatheringSession> gatherings){
            return gatherings.Count > 1;
        }

        private void CreateCombinedGathering(List<GatheringSession> gatherings){
            StringBuilder strBuilder = new StringBuilder();
            foreach (var gathering in gatherings){
                strBuilder.Append(gathering.Gatherings + " /");
            }
            GatheringsName.Text = strBuilder + " [Combined]";
            GatheringDate.Text = gatherings[0].Date.ToString("MMM dd, yyyy");
        }

        private void AddBrethrenToListView(BrethrenBasic brethren){
            _brethrenInfoList.Add(new BrethrenListViewCheckDTO{Brethren = brethren});
        }

        private void Logged_Click(object sender, RoutedEventArgs e){
            foreach (var b in _brethrenInfoList){
                LogBrethren(b);
            }
            RemoveBrethrenFromTheSelection(_brethrenInfoList.ToList());
            _brethrenInfoList.Clear();
        }

            private void LogBrethren(BrethrenListViewCheckDTO brethrenInfo){
                var brethren = brethrenInfo.Brethren;
                bool isLate = brethrenInfo.IsLate;
                foreach (var gathering in _gatherings){
                    LogBrethrenToGatherings(brethren, isLate, gathering);
                }
            }

                private void LogBrethrenToGatherings(BrethrenBasic brethren, bool isLate, GatheringSession gathering){
                    var dateNow = RemoveHoursAndMinute(DateTime.Now);
                    var log = CreateAttendanceLog(brethren.Id, dateNow, isLate);
                    _attendanceLogger.Logbrethren(gathering.Id, log);
                }

                    private DateTime RemoveHoursAndMinute(DateTime date){
                        return new DateTime(date.Year, date.Month, date.Day);
                    }

                    private AttendanceLog CreateAttendanceLog(long brethrenId, DateTime dateNow, bool isLate){
                        return new AttendanceLog{BrethrenId = brethrenId, DateTime = dateNow, IsLate = isLate};
                    }

            private void RemoveBrethrenFromTheSelection(List<BrethrenListViewCheckDTO> brethrenInfoList){
                foreach (var b in brethrenInfoList){
                    _brethrenCollection.Remove(b.Brethren);
                }
            }

        private void Remove_Click(object sender, RoutedEventArgs e){
            var brethrenInfo = (BrethrenListViewCheckDTO) BrethrenListView.SelectedValue;
            _brethrenInfoList.Remove(brethrenInfo);
        }
    }
}
