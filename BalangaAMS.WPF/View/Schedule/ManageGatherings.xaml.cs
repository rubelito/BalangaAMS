using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace BalangaAMS.WPF.View.Schedule
{
    /// <summary>
    /// Interaction logic for ManageSchedule.xaml
    /// </summary>
    public partial class ManageGatherings
    {
        private readonly IChurchGatheringManager _sessionManager;
        private readonly IChurchGatheringRetriever _sessionRetriever;

        private ObservableCollection<Gathering> _gatheringList;
        private DateTime _datenow;
        private bool _isControlReady;

        public ManageGatherings(DateTime dateNow)
        {
            _sessionManager = UnityBootstrapper.Container.Resolve<IChurchGatheringManager>();
            _sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            InitializeComponent();

            _datenow = dateNow;

            ScheduleView.AppointmentDeleting += ScheduleViewOnAppointmentDeleting;
            ScheduleView.AppointmentCreating += ScheduleViewOnAppointmentCreating;
            ScheduleView.AppointmentEditing += ScheduleViewOnAppointmentEditing;

            Loaded += ManageSchedule_Loaded;
            HighlightNextAndPreviousMonth();
        }

        private void ManageSchedule_Loaded(object sender, RoutedEventArgs e){
            _isControlReady = true;
            PopulateSlotWithGatherings();
        }

        private void PopulateSlotWithGatherings()
        {
            var appointmentGenerator = new AppointmentGenerator(_sessionRetriever, _datenow);
            _gatheringList = appointmentGenerator.GetPreviousNowNextGatherings();
            ScheduleView.AppointmentsSource = _gatheringList;
            ScheduleView.Commit();
        }

        private void ScheduleViewOnAppointmentEditing(object sender, AppointmentEditingEventArgs e)
        {
            var gathering = e.Appointment as Gathering;
            var editSchedule = new EditSchedule(gathering, _sessionRetriever, _sessionManager);
            editSchedule.ShowDialog();
            if (editSchedule.IsEdited())
            {
                _gatheringList.Remove(gathering);
                _gatheringList.Add(editSchedule.GetEditedGathering());
            }
        }

        private void ScheduleViewOnAppointmentCreating(object sender, AppointmentCreatingEventArgs e)
        {
            var slot = ScheduleView.SelectedSlot;
            if (slot != null){
                var createSchedule = new CreateSchedule(_sessionManager, slot.Start);
                createSchedule.ShowDialog();
                if (createSchedule.IsCanceled() == false){
                    var gathering = createSchedule.GetCreatedGathering();
                    ((ObservableCollection<Gathering>) ScheduleView.AppointmentsSource).Add(gathering);
                }
                e.Cancel = true;
            }
        }

        private void ScheduleViewOnAppointmentDeleting(object sender, AppointmentDeletingEventArgs e)
        {
            var gathering = e.Appointment as Gathering;
            if (gathering != null)
            {
                var isSucessful = DeleteGatheringsOnDataBase(Convert.ToInt32(gathering.UniqueId));
                e.Cancel = !isSucessful;
            }
        }

        private bool DeleteGatheringsOnDataBase(long id)
        {
            var session = _sessionRetriever.GetGatheringById(id);
            _sessionManager.RemoveGathering(session);
            return _sessionManager.IsRemovingSuccessful();
        }

        private void HighlightNextAndPreviousMonth()
        {
            var previousMonth = _datenow.AddMonths(-1);
            var previousMonthSlot = new List<Slot>(DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            for (int x = 1; x <= previousMonthSlot.Capacity; x++)
            {
                previousMonthSlot.Add(new Slot(new DateTime(previousMonth.Year, previousMonth.Month, x, 0, 1, 1),
                    new DateTime(previousMonth.Year, previousMonth.Month, x, 1, 1, 1)));
            }
            var nextMonth = _datenow.AddMonths(1);
            var nextMonthSlot = new List<Slot>(DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            for (int x = 1; x <= nextMonthSlot.Capacity; x++)
            {
                nextMonthSlot.Add(new Slot(new DateTime(nextMonth.Year, nextMonth.Month, x, 0, 1, 1),
                    new DateTime(nextMonth.Year, nextMonth.Month, x, 1, 1, 1)));
            }
            previousMonthSlot.AddRange(nextMonthSlot);
            ScheduleView.SpecialSlotsSource = new ObservableCollection<Slot>(previousMonthSlot);
        }

        private void ScheduleView_OnShowDialog(object sender, ShowDialogEventArgs e)
        {
            if (e.DialogViewModel is ConfirmDialogViewModel)
            {
                var yesorno = MessageBox.Show("Are you sure do you want to Delete this Schedule?", "Delete Schedule",
                    MessageBoxButton.YesNo);
                if (yesorno == MessageBoxResult.Yes)
                {
                    e.DefaultDialogResult = true;
                }
                e.Cancel = true;
            }
            e.Cancel = true;
        }

        private void ScheduleView_OnVisibleRangeChanged(object sender, EventArgs e){
            if (_isControlReady){
                _datenow = ScheduleView.CurrentDate;
                PopulateSlotWithGatherings();
                HighlightNextAndPreviousMonth();
            }
        }
    }
}