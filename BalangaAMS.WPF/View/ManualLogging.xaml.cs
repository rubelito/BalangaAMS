using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
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
        private readonly IChurchIdManager _churchIdManager;
        private readonly IOtherLocalManager _otherLocalManager;
        private ObservableCollection<BrethrenBasic> _brethrenCollection;
        private ObservableCollection<ChurchId> _churchIdCollection; 
        private readonly ObservableCollection<BrethrenListViewCheckDTO> _brethrenInfoList;
        private List<GatheringSession> _gatherings;

        public ManualLogging(){
            InitializeComponent();
            _attendanceLogger = UnityBootstrapper.Container.Resolve<IAttendanceLogger>();
            _attendanceRetriever = UnityBootstrapper.Container.Resolve<IAttendanceRetriever>();
            _churchIdManager = UnityBootstrapper.Container.Resolve<IChurchIdManager>();
            _otherLocalManager = UnityBootstrapper.Container.Resolve<IOtherLocalManager>();
            _brethrenInfoList = new ObservableCollection<BrethrenListViewCheckDTO>();
            BrethrenListView.DataContext = _brethrenInfoList;
            NoTimeRbtn.IsChecked = true;
            TimePicker.IsEnabled = false;
            TimePicker.SelectedValue = DateTime.Now;
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
                LoadOtherLocal();
                _brethrenInfoList.Clear();
            }
        }

        private void LoadBrethrenToAutoCompletebox(List<BrethrenBasic> brethrenList){
            _brethrenCollection =
                new ObservableCollection<BrethrenBasic>(brethrenList);
            SearchGroupBox.DataContext = CollectionViewSource.GetDefaultView(_brethrenCollection);
        }

        private void LoadOtherLocal(){
            _churchIdCollection = new ObservableCollection<ChurchId>(_churchIdManager.GetAllChurchIds());

            SearchOtherGroupBox.DataContext = CollectionViewSource.GetDefaultView(_churchIdCollection);
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
            var d = new BrethrenListViewCheckDTO();
            d.Brethren = brethren;
            d.IsOtherLocal = false;
            SetTime(d);

            _brethrenInfoList.Add(d);
        }

        private void Logged_Click(object sender, RoutedEventArgs e){
            foreach (var b in _brethrenInfoList){
                if (b.IsOtherLocal){
                    LogOtherLocal(b);                  
                }
                else{
                    LogBrethren(b);
                }
            }
            RemoveBrethrenFromTheSelection(_brethrenInfoList.ToList());
            _brethrenInfoList.Clear();
        }

            private void LogBrethren(BrethrenListViewCheckDTO brethrenInfo){
                foreach (var gathering in _gatherings){
                    LogBrethrenToGatherings(brethrenInfo, gathering);
                }
            }

            private void LogOtherLocal(BrethrenListViewCheckDTO d){
                foreach (var gathering in _gatherings){
                    LogOtherLocalToGathering(d, gathering);
                }
            }

                private void LogBrethrenToGatherings(BrethrenListViewCheckDTO d, GatheringSession gathering){
                    var logTime = d.HasTime ? d.LogTime : RemoveHoursAndMinute(DateTime.Now);
                    var log = CreateAttendanceLog(d.Brethren.Id, logTime, d.IsLate);
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

        private void LogOtherLocalToGathering(BrethrenListViewCheckDTO otherLocal, GatheringSession gathering){
            var l = new OtherLocalLog();
            l.ChurchId = otherLocal.Brethren.ChurchId;
            l.IsLate = otherLocal.IsLate;
            l.DateTime = otherLocal.LogTime;

            _otherLocalManager.LogAttendance(l, gathering.Id);
        }

        private void Remove_Click(object sender, RoutedEventArgs e){
            var brethrenInfo = (BrethrenListViewCheckDTO) BrethrenListView.SelectedValue;
            _brethrenInfoList.Remove(brethrenInfo);
        }

        private void NoTimeRbtn_OnChecked(object sender, RoutedEventArgs e){
            TimePicker.IsEnabled = false;
        }

        private void WithTimeRbtn_OnChecked(object sender, RoutedEventArgs e){
            TimePicker.IsEnabled = true;
        }

        private void AutoCompleteBoxOtherLocalChurchId_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            var Id = AutoCompleteBoxOtherLocalChurchId.SelectedItem as ChurchId;
            if (Id != null)
            {
                AutoCompleteBoxOtherLocalChurchId.SearchText = string.Empty;
                AddOtherLocalToList(Id.Code);
            }
        }

        private void AutoCompleteBoxOtherLocalChurchId_OnKeyDown(object sender, KeyEventArgs e){
            if (e.Key == Key.Enter && _gatherings != null){
                var id = AutoCompleteBoxOtherLocalChurchId.SelectedItems as ChurchId;
                if (id != null){
                    AutoCompleteBoxOtherLocalChurchId.SearchText = string.Empty;
                    AddOtherLocalToList(id.Code);
                }                  
                else{
                    string newChurchId = AutoCompleteBoxOtherLocalChurchId.SearchText.ToUpper();
                    if (!string.IsNullOrWhiteSpace(newChurchId)){
                        AutoCompleteBoxOtherLocalChurchId.SearchText = string.Empty;
                        CreateNewEntryOfOtherLocal(newChurchId);
                        AddOtherLocalToList(newChurchId);
                    }
                }
            }
        }

        private void CreateNewEntryOfOtherLocal(string churchId)
        {
            var newId = new ChurchId();
            newId.Code = churchId;
            _churchIdManager.AddChurchId(newId);
        }

        private void AddOtherLocalToList(string churchId){
            BrethrenBasic b = new BrethrenBasic();
            b.ChurchId = churchId;
            b.Name = "Other Local";

            BrethrenListViewCheckDTO d = new BrethrenListViewCheckDTO();
            d.Brethren = b;
            d.IsOtherLocal = true;
            SetTime(d);
            
            _brethrenInfoList.Add(d);
        }

        private void SetTime(BrethrenListViewCheckDTO d)
        {
            if (WithTimeRbtn.IsChecked.HasValue && WithTimeRbtn.IsChecked.Value)
            {
                d.HasTime = true;
                d.LogTime = TimePicker.SelectedValue.HasValue ? TimePicker.SelectedValue.Value : DateTime.Now;
            }
            else{
                d.HasTime = false;
                d.LogTime = RemoveHoursAndMinute(DateTime.Now);
            }
        }
    }
}
