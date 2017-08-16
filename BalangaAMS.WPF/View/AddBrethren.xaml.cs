using System;
using System.Linq;
using System.Windows;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for AddBrethren.xaml
    /// </summary>
    public partial class AddBrethren
    {
        private readonly IBrethrenManager _brethrenManager;
        private bool _isCanceled;
        private BrethrenBasic _createdBrethren;

        public AddBrethren(IBrethrenManager brethrenManager)
        {
            _brethrenManager = brethrenManager;
            InitializeComponent();
            _createdBrethren = new BrethrenBasic();
            _createdBrethren.BrethrenFull = new BrethrenFull();
            DataContext = _createdBrethren;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (IsBrethrenAlreadyExist(_createdBrethren))
            {
                MessageBox.Show("Record already exist in Database");
            }
            else
            {
                CreateBrethrenOnDatabase(_createdBrethren);
                _isCanceled = false;
                Close();
            }            
        }

            private bool IsBrethrenAlreadyExist(BrethrenBasic brethren){
                return _brethrenManager.FindBrethren(b => b.ChurchId == brethren.ChurchId).FirstOrDefault() != null;
            }

            private void CreateBrethrenOnDatabase(BrethrenBasic brethren){
                brethren.LastStatusUpdate = DateTime.Now;
                _brethrenManager.AddBrethren(brethren);
            }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _isCanceled = true;
            Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _createdBrethren = new BrethrenBasic();
            _createdBrethren.BrethrenFull = new BrethrenFull();
            DataContext = _createdBrethren;
        }

        public bool IsCanceled()
        {
            return _isCanceled;
        }

        public BrethrenBasic GetCreatedBrethren()
        {
            return _createdBrethren;
        }
    }

}
