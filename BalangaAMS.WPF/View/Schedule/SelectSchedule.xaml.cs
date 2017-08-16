using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace BalangaAMS.WPF.View.Schedule
{
    /// <summary>
    /// Interaction logic for SelectSchedule.xaml
    /// </summary>
   
    public partial class SelectSchedule
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;

        private ObservableCollection<Gathering> _gatheringList;
        private bool _isCanceled = true;
        private GatheringSession _selectedGathering;
        private List<GatheringSession> _selectedGatherings; 
        private readonly DateTime _currentDate;

        public SelectSchedule(DateTime currentDate)
        {
            _currentDate = currentDate; 
            _sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            Initialize();                     
        }

        private void Initialize()
        {            
            InitializeComponent();
            _selectedGatherings = new List<GatheringSession>();
            ScheduleView.AppointmentEditing += ScheduleViewOnAppointmentEditing;
            ScheduleView.CurrentDate = _currentDate;
            HighlightNextAndPreviousMonth();
        }        

        private void PopulateSlotWithGatherings()
        {
            var appointmentGenerator = new AppointmentGenerator(_sessionRetriever, _currentDate);
            _gatheringList = appointmentGenerator.GetPreviousNowNextGatherings();
            ScheduleView.AppointmentsSource = _gatheringList;
            ScheduleView.Commit();
        }

        private void SelectSchedule_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateSlotWithGatherings();
        }

        private void HighlightNextAndPreviousMonth()
        {
            var previousMonth = _currentDate.AddMonths(-1);
            var previousMonthSlot = new List<Slot>(DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            for (int x = 1; x <= previousMonthSlot.Capacity; x++)
            {
                previousMonthSlot.Add(new Slot(new DateTime(previousMonth.Year, previousMonth.Month, x, 0, 1, 1),
                    new DateTime(previousMonth.Year, previousMonth.Month, x, 1, 1, 1)));
            }
            var nextMonth = _currentDate.AddMonths(1);
            var nextMonthSlot = new List<Slot>(DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            for (int x = 1; x <= nextMonthSlot.Capacity; x++)
            {
                nextMonthSlot.Add(new Slot(new DateTime(nextMonth.Year, nextMonth.Month, x, 0, 1, 1),
                    new DateTime(nextMonth.Year, nextMonth.Month, x, 1, 1, 1)));
            }
            previousMonthSlot.AddRange(nextMonthSlot);
            ScheduleView.SpecialSlotsSource = new ObservableCollection<Slot>(previousMonthSlot);
        }

        private void ScheduleViewOnAppointmentEditing(object sender, AppointmentEditingEventArgs e){
            if (SelectGatherings.Visibility == Visibility.Visible)
                return;

            var g = e.Appointment as Gathering;
            if (g.IsStarted == false){
                MessageBox.Show("This Gathering is not mark as Started", "Cannot select this Gathering",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var yesorno = MessageBox.Show(g.Subject + " : " + g.Start.ToString("MMM dd, yyyy") +
                                          Environment.NewLine +
                                          "Are you sure, do you want to start logging this Gathering ?",
                "Start this Gathering",
                MessageBoxButton.YesNo);

            if (yesorno == MessageBoxResult.Yes){
                _selectedGathering = _sessionRetriever.GetGatheringById(Convert.ToInt32(g.UniqueId));
                _isCanceled = false;
                Close();
            }
        }

        private void ScheduleView_OnShowDialog(object sender, ShowDialogEventArgs e)
        {
            e.Cancel = true;
        }

        public GatheringSession GetSelectedGathering()
        {
            return _selectedGathering;
        }

        public List<GatheringSession> GetSelectedGatherings()
        {
            foreach (Gathering appointment in ScheduleView.SelectedAppointments)
            {
                var gathering = _sessionRetriever.GetGatheringById(Convert.ToInt32(appointment.UniqueId));
                _selectedGatherings.Add(gathering);
            }
            return _selectedGatherings;
        }

        private bool IsGatheringIsNotStarted(Gathering appointment){
            return !appointment.IsStarted;
        }

        public bool IsCanceled()
        {
            return _isCanceled;
        }

        private void SelectGatherings_Click(object sender, RoutedEventArgs e)
        {
            if (!HasSelectedGatherings())
            {
                MessageBox.Show("You Must select gatherings");
                return;
            }

            if (!CanSelectNotStartedGathering)
                foreach (Gathering appointment in ScheduleView.SelectedAppointments){
                    if (IsGatheringIsNotStarted(appointment)){
                        MessageBox.Show(
                            "You Must mark Gathering as Started in order to start logging with this gathering",
                            "Cannot select Gathering", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

            _isCanceled = false;
            Close();
        }

        private bool HasSelectedGatherings()
        {
            var gatherings = ScheduleView.SelectedAppointments;
            if (gatherings.Count == 0)
                return false;
                return true;
        }

        public bool CanSelectNotStartedGathering { get; set; }
    }
}
