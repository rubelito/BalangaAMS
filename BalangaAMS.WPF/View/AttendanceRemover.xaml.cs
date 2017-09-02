using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using BalangaAMS.WPF.View.Schedule;
using Microsoft.Practices.Unity;
using MessageBox = System.Windows.MessageBox;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for AttendanceRemover.xaml
    /// </summary>
    public partial class AttendanceRemover
    {
        private readonly IAttendanceRetriever _attendanceRetriever;
        private readonly IAttendanceLogRetriever _attendanceLogRetriever;
        private readonly IOtherLocalManager _otherLocalManager;
 
        private readonly ObservableCollection<BrethrenRemoveCheckDTO> _brethrenInfoList;
        private ICollectionView _brethrenInfoListView;
        private List<GatheringSession> _gatherings;

        public AttendanceRemover(){
            InitializeComponent();
            _attendanceRetriever = UnityBootstrapper.Container.Resolve<IAttendanceRetriever>();
            _attendanceLogRetriever = UnityBootstrapper.Container.Resolve<IAttendanceLogRetriever>();
            _otherLocalManager = UnityBootstrapper.Container.Resolve<IOtherLocalManager>();
            _brethrenInfoList = new ObservableCollection<BrethrenRemoveCheckDTO>();
            
            _brethrenInfoListView = CollectionViewSource.GetDefaultView(_brethrenInfoList);
            _brethrenInfoListView.SortDescriptions.Add(new SortDescription("Brethren.Name", ListSortDirection.Ascending));
            BrethrenListView.DataContext = _brethrenInfoListView;
        }

        private void SelectGatheringButton_Click(object sender, RoutedEventArgs e){
            var selectGathering = new SelectSchedule(DateTime.Now);
            selectGathering.CanSelectNotStartedGathering = false;
            selectGathering.ShowDialog();
            if (!selectGathering.IsCanceled()){
                _gatherings = selectGathering.GetSelectedGatherings();
                SetGatheringBoxInfo(_gatherings);
                var attendedBrethren = _attendanceRetriever.GetBrethrenWhoAttendedThisGathering(_gatherings[0]);

                var otherLocals = _attendanceRetriever.GetOtherLocalWhoAttendedThisGathering(_gatherings[0]);

                _brethrenInfoList.Clear();
                AddBrethrenToListBox(attendedBrethren);
                AddOtherLocalToListBox(otherLocals);
            }
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

        private void AddBrethrenToListBox(List<BrethrenBasic> brethrenList){
            foreach (var brethren in brethrenList){
                _brethrenInfoList.Add(new BrethrenRemoveCheckDTO {Brethren = brethren, IsOtherLocal = false });
            }
        }

        private void AddOtherLocalToListBox(List<string> otherLocals){
            foreach (string o in otherLocals){
                BrethrenBasic b = new BrethrenBasic();
                b.ChurchId = o;
                b.Name = "Other Local";

                _brethrenInfoList.Add(new BrethrenRemoveCheckDTO {Brethren = b, IsOtherLocal = true });
            }
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e){
            _brethrenInfoListView.Filter = BrethrenFilter;
        }

        private bool BrethrenFilter(object item){
            var dto = item as BrethrenRemoveCheckDTO;

            if (dto.IsOtherLocal){
                return dto != null &&
                       !string.IsNullOrWhiteSpace(dto.Brethren.ChurchId) &&
                       dto.Brethren.ChurchId.IndexOf(SearchName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return dto != null &&
                   !string.IsNullOrWhiteSpace(dto.Brethren.Name) &&
                   dto.Brethren.Name.IndexOf(SearchName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RemoveLog_Click(object sender, RoutedEventArgs e){
            if (!_brethrenInfoList.Any(b => b.WillRemove)){
                MessageBox.Show("Please Check Brethren to Remove their attendance");
                return;
            }

            var selectedBrethren = _brethrenInfoList.Where(b => b.WillRemove).ToList();

            foreach (var b in selectedBrethren){
                RemoveAttendance(b);
                _brethrenInfoList.Remove(b);
            }
            RemoveBrethrenInListView(selectedBrethren);
            
            MessageBox.Show("Attendance Removed");
        }

            private void RemoveAttendance(BrethrenRemoveCheckDTO brethrenInfo){
                var brethren = brethrenInfo.Brethren;
                foreach (var gathering in _gatherings){
                    if (brethrenInfo.IsOtherLocal){
                        _otherLocalManager.RemoveAttendanceLog(brethren.ChurchId, gathering.Id);
                    }
                    else{
                        _attendanceLogRetriever.RemoveAttendanceLog(brethren.Id, gathering.Id);
                    } 
                }
            }

        private void RemoveBrethrenInListView(List<BrethrenRemoveCheckDTO> brethrenInfoList){
            foreach (var b in brethrenInfoList){
                _brethrenInfoList.Remove(b);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e){
            Close();
        }
    }
}
