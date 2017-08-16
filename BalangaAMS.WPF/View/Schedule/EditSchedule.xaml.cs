using System;
using System.Windows;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.WPF.View.Schedule
{
    /// <summary>
    /// Interaction logic for EditSchedule.xaml
    /// </summary>
    public partial class EditSchedule
    {
        private readonly Gathering _gathering;
        private readonly IChurchGatheringManager _sessionManager;
        private readonly GatheringSession _session;
        private bool _isEdited;
        
        public EditSchedule(Gathering gathering, IChurchGatheringRetriever sessionRetriever, IChurchGatheringManager sessionManager)
        {
            InitializeComponent();           
            _gathering = gathering;
            _sessionManager = sessionManager;
            _session = sessionRetriever.GetGatheringById(Convert.ToInt32(gathering.UniqueId));
            DataContext = _gathering;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _gathering.Subject = ((Gatherings) CboGatheringType.SelectedValue).ToString();
            _gathering.Start = CboDate.SelectedValue.HasValue ? CboDate.SelectedValue.Value : _gathering.Start;
            _gathering.End = _gathering.Start.AddMinutes(1);
            _gathering.IsStarted = IsStarted.IsChecked == true;
            UpdateGatheringsSessionOnDatabase(_gathering);

            _isEdited = true;
            Close();
        }

        private void UpdateGatheringsSessionOnDatabase(Gathering gathering)
        {
            _session.Gatherings = (Gatherings)Enum.Parse(typeof(Gatherings), gathering.Subject);
            _session.Date = gathering.Start;
            _session.IsStarted = _gathering.IsStarted;
            _sessionManager.UpdateGathering(_session);
        }

        public bool IsEdited()
        {
            return _isEdited;
        }

        public Gathering GetEditedGathering()
        {
            return _gathering;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _isEdited = false;
            Close();
        }
    }
}
