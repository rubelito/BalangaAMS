using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.DTO;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for FingerPrintEnrollment.xaml
    /// </summary>
    public partial class FingerPrintEnrollment
    {
        private readonly IBrethrenManager _brethrenManager;
        private ICollectionView _fPCollectionView;
        private readonly int _daysToConsiderNewlyBaptised;

        public FingerPrintEnrollment(){
            InitializeComponent();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _daysToConsiderNewlyBaptised =
                Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
            LoadAndBindBrethren();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e){
            RadioAll.IsChecked = true;
        }

        private void LoadAndBindBrethren(){
            var fpList = new List<FPViewDTO>();
            var brethrenList =
                _brethrenManager.FindBrethren(b => b.LocalStatus == LocalStatus.Present_Here);
            foreach (var b in brethrenList){
                fpList.Add(new FPViewDTO{Brethren = b});
            }

            AddNewlyBaptisedGroup(fpList);

            var brethrenCollection = new ObservableCollection<FPViewDTO>(fpList);
            _fPCollectionView = CollectionViewSource.GetDefaultView(brethrenCollection);
            _fPCollectionView.SortDescriptions.Add(new SortDescription("GroupName", ListSortDirection.Ascending));
            _fPCollectionView.SortDescriptions.Add(new SortDescription("Brethren.Name", ListSortDirection.Ascending));
            BrethrenListView.DataContext = _fPCollectionView;
        }

        private void AddNewlyBaptisedGroup(List<FPViewDTO> fpList){
            foreach (var fpViewDTO in fpList){
                if (fpViewDTO.Brethren.Group != null)
                    fpViewDTO.GroupName = fpViewDTO.Brethren.Group.GroupName;
                else{
                    if (_brethrenManager.IsNewlyBaptised(fpViewDTO.Brethren, _daysToConsiderNewlyBaptised, DateTime.Now))
                    fpViewDTO.GroupName = "Newly Bapstized";
                }
            }
        }

        private void RadioAll_Checked(object sender, RoutedEventArgs e){
            FilterBrethren(SearchName.Text);
        }

        private void RadioHave_Checked(object sender, RoutedEventArgs e){
            FilterBrethren(SearchName.Text);
        }

        private void RadioNo_Checked(object sender, RoutedEventArgs e){
            FilterBrethren(SearchName.Text);
        }

        private void SearchName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e){
            FilterBrethren(SearchName.Text);
        }

        private void FilterBrethren(string searchFilter){
            if (RadioAll.IsChecked == true)
                _fPCollectionView.Filter = b =>{
                    var fp = b as FPViewDTO;
                    return fp.Brethren != null &&
                           fp.Brethren.Name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                };

            if (RadioHave.IsChecked == true)
                _fPCollectionView.Filter = b =>{
                    var fp = b as FPViewDTO;
                    return fp.Brethren != null && fp.Brethren.FingerPrint != null &&
                           fp.Brethren.Name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            if (RadioNo.IsChecked == true)
                _fPCollectionView.Filter = b =>{
                    var fp = b as FPViewDTO;
                    return fp.Brethren != null && fp.Brethren.FingerPrint == null &&
                           fp.Brethren.Name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                };
        }

        private void Enroll_Click(object sender, RoutedEventArgs e){
            var brethrenToEnroll = _fPCollectionView.CurrentItem as FPViewDTO;
            if (brethrenToEnroll == null)
                return;
            var enrollForm = new EnrollForm(brethrenToEnroll.Brethren);
            enrollForm.Owner = this;
            enrollForm.ShowDialog();
            if (enrollForm.IsCanceled() == false){
                _brethrenManager.Updatebrethren(brethrenToEnroll.Brethren);
                FilterBrethren(SearchName.Text);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e){
            Close();
        }
    }
}
