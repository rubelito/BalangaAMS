using System;
using System.Windows;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.WPF.View.Schedule
{
    /// <summary>
    /// Interaction logic for CreateSchedule.xaml
    /// </summary>
    public partial class CreateSchedule
    {
        private readonly IChurchGatheringManager _sessionManager;
        private readonly DateTime _datenow;
        private Gathering _gathering;
        private bool _isCanceled;

        public CreateSchedule(IChurchGatheringManager sessionManager, DateTime datenow)
        {          
            InitializeComponent();
            _sessionManager = sessionManager;
            _datenow = datenow;
            DateBlock.Text = _datenow.ToString("MMMM dd, yyyy");
        }

        public Gathering GetCreatedGathering()
        {
            return _gathering;
        }

        public bool IsCanceled()
        {
            return _isCanceled;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            _gathering = new Gathering();
            _gathering.Subject = ((Gatherings)CboGatheringType.SelectedValue).ToString();
            _gathering.Start =
                _datenow.AddMinutes(TimePicker.SelectedTime.HasValue ? TimePicker.SelectedTime.Value.TotalMinutes : 1);
            _gathering.End = _datenow.AddHours(1);
            _gathering.IsStarted = false;
            CreateGatheringSessionOnDatabase(_gathering);

            _isCanceled = false;
            Close();
        }

        private void CreateGatheringSessionOnDatabase(Gathering gathering)
        {
            Gatherings gatherings =  (Gatherings) Enum.Parse(typeof (Gatherings), gathering.Subject);
            _sessionManager.CreateGathering(gatherings, gathering.Start);
            gathering.UniqueId = _sessionManager.ReturnNewlyCreatedGathering().Id.ToString();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _isCanceled = true;
            Close();
        }
    }
}
