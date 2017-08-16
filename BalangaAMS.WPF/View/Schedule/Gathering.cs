using System.Windows.Media;
using Telerik.Windows.Controls.ScheduleView;

namespace BalangaAMS.WPF.View.Schedule
{
    public class Gathering : Appointment
    {
        private bool _isStarted;
        private bool _hasAttendees;
        private SolidColorBrush _backColor;

        public bool IsStarted
        {
            get
            {
                return Storage<Gathering>()._isStarted;
            }
            set
            {
                var storage = Storage<Gathering>();
                storage._isStarted = value;
                if (IsStartedAndDontHaveAttendees())
                    _backColor = new SolidColorBrush(Colors.LightGreen);
                else if (IsStartedAndHasAttendees()){
                    _backColor = new SolidColorBrush(Colors.LightBlue);
                }
                else{
                    _backColor = new SolidColorBrush(Colors.LightGray);
                }
                OnPropertyChanged(() => IsStarted);
                OnPropertyChanged(() => BackColor);
            }
        }

        private bool IsStartedAndDontHaveAttendees(){
            return _isStarted && !_hasAttendees;
        }

        private bool IsStartedAndHasAttendees(){
            return _isStarted && _hasAttendees;
        }
        
        public SolidColorBrush BackColor
        {
            get
            {
                return Storage<Gathering>()._backColor;
            }
        }

        public override IAppointment Copy()
        {
            var newAppointment = new Gathering();
            newAppointment.CopyFrom(this);
            return newAppointment;
        }

        public override void CopyFrom(IAppointment other)
        {
            var gathering = other as Gathering;
            if (gathering != null)
            {
                _isStarted = gathering._isStarted;
                _backColor = gathering._backColor;
            }
            base.CopyFrom(other);
        }

        public bool HasAttendees
        {
            get
            {
                return Storage<Gathering>()._hasAttendees;
            }
            set
            {
                var storage = Storage<Gathering>();
                storage._hasAttendees = value;
                if (IsStartedAndHasAttendees())
                    _backColor = new SolidColorBrush(Colors.LightBlue);
                OnPropertyChanged(() => HasAttendees);
                OnPropertyChanged(() => BackColor);
            }
        }
    }
}
