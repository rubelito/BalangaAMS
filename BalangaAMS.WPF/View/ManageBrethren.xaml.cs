using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.HelperClass;
using BalangaAMS.WPF.View.Report;
using Microsoft.Practices.Unity;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace BalangaAMS.WPF.View
{
    public partial class ManageBrethren
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly ICollectionView _brethrenView;
        private readonly IBrethrenManager _brethrenUpdater;
        private readonly IStatusIdentifier _statusIdentifier;
        private readonly IImageService _imageService;
        private readonly int _daysToConsiderNewlyBaptised;

        public ManageBrethren(){
            InitializeComponent();

            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _brethrenUpdater = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _imageService = UnityBootstrapper.Container.Resolve<IImageService>();
            _statusIdentifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            _daysToConsiderNewlyBaptised =
                Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);

            var brethrenList =
                new ObservableCollection<BrethrenBasic>(_brethrenManager.GetAllBrethren().OrderBy(b => b.Name).ToList());
            _brethrenView = CollectionViewSource.GetDefaultView(brethrenList);
            _brethrenView.CurrentChanged += BrethrenViewOnCurrentChanged;

            var groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            CboGroup.DataContext = new ObservableCollection<Group>(groupManager.Getallgroup());
            DataContext = _brethrenView;
            _brethrenView.MoveCurrentToPosition(0);
            AddNewlyBaptisedGroupAndNoGroup();
        }


        private void AddNewlyBaptisedGroupAndNoGroup(){
            (CboGroup.DataContext as ObservableCollection<Group>).Add(new Group(){GroupName = "Newly Baptised"});
            (CboGroup.DataContext as ObservableCollection<Group>).Add(new Group(){GroupName = "No Group"});
        }

        private void Edit_Click(object sender, RoutedEventArgs e){
            if (_brethrenView.CurrentItem == null){
                MessageBox.Show("Please select Brethren from the List");
                return;
            }
            Save.IsEnabled = true;
            Cancel.IsEnabled = true;
            Edit.IsEnabled = false;
            Remove.IsEnabled = false;
            Add.IsEnabled = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e){
            SaveChanges();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e){
            Edit.IsEnabled = true;
            Save.IsEnabled = false;
            Cancel.IsEnabled = false;
            Remove.IsEnabled = true;
            Add.IsEnabled = true;
        }

        private void SaveChanges(){
            try{
                var brethren = _brethrenView.CurrentItem as BrethrenBasic;
                _brethrenUpdater.Updatebrethren(brethren);
                Edit.IsEnabled = true;
                Save.IsEnabled = false;
                Cancel.IsEnabled = false;
                Remove.IsEnabled = true;
                Add.IsEnabled = true;
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Error Saving", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void Add_Click(object sender, RoutedEventArgs e){
            var addbrethren = new AddBrethren(_brethrenManager);
            addbrethren.ShowDialog();
            if (addbrethren.IsCanceled() == false){
                var brethren = addbrethren.GetCreatedBrethren();
                ((ObservableCollection<BrethrenBasic>) _brethrenView.SourceCollection).Add(brethren);
                MessageBox.Show("Record Added");
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e){
            if (_brethrenView.CurrentItem == null){
                MessageBox.Show("Please select Brethren from the List");
                return;
            }

            RemoveBrethren();
        }

        private void RemoveBrethren(){
            var yesorno = MessageBox.Show("Are you sure do you want to Delete this record?", "Delete Record",
                MessageBoxButton.YesNo);
            if (yesorno == MessageBoxResult.Yes){
                var brethren = (BrethrenBasic) _brethrenView.CurrentItem;
                _imageService.RemovePicture(brethren.Id);
                _brethrenManager.DeleteBrethren(brethren);
                ((ObservableCollection<BrethrenBasic>) _brethrenView.SourceCollection).Remove(brethren);
            }
        }

        private void BrethrenViewOnCurrentChanged(object sender, EventArgs eventArgs){
            var brethren = _brethrenView.CurrentItem as BrethrenBasic;
            if (brethren == null) return;
            Image.Source = ImageToBitmap.ConvertToBitmapImage(_imageService.GetPicture(brethren.Id));
            Resources["EnableAddPicture"] = _imageService.HasPicture() == false;
        }

        private void CboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (IsAllOrChurchIdOrNameIsSelected()) return;
            if (_brethrenView != null)
                _brethrenView.Filter = b =>{
                    var brethren = b as BrethrenBasic;
                    return brethren != null;
                };
        }

        private bool IsAllOrChurchIdOrNameIsSelected(){
            return CboCategory.SelectedIndex > 2;
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e){
            _brethrenView.Filter = b =>{
                var brethren = b as BrethrenBasic;
                return brethren != null &&
                       brethren.Name.IndexOf(SearchName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };
        }

        private void SearchChurchId_TextChanged(object sender, TextChangedEventArgs e){
            _brethrenView.Filter = b =>{
                var brethren = b as BrethrenBasic;
                return brethren != null &&
                       !string.IsNullOrWhiteSpace(brethren.ChurchId) &&
                       brethren.ChurchId.IndexOf(SearchChurchId.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };
        }

        private void CboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboMonth)){
                _brethrenView.Filter = b =>{
                    var brethren = (BrethrenBasic) b;
                    var month = Convert.ToInt32((MonthofYear) CboMonth.SelectedItem);
                    return brethren.BrethrenFull.DateofBaptism.HasValue &&
                           brethren.BrethrenFull.DateofBaptism.Value.Month == month &&
                           brethren.LocalStatus == LocalStatus.Present_Here;
                };
            }
        }

        private void CboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboGroup)){
                _brethrenView.Filter = b =>{
                    var brethren = (BrethrenBasic) b;
                    var group = (Group) CboGroup.SelectedItem;

                    if (group.GroupName == "Newly Baptised"){
                        return brethren.Group == null && brethren.BrethrenFull.DateofBaptism.HasValue &&
                               _brethrenManager.IsNewlyBaptised(brethren, _daysToConsiderNewlyBaptised, DateTime.Now);
                    }
                    if (group.GroupName == "No Group"){
                        return brethren.Group == null;
                    }
                    return brethren.Group != null && brethren.Group.Id == group.Id;
                };
            }
        }

        private bool HasSelectedItem(ComboBox comboBox){
            return comboBox.SelectedIndex >= 0;
        }

        private void CboGender_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboGender)){
                _brethrenView.Filter = b =>{
                    var brethren = (BrethrenBasic) b;
                    var gender = (Gender) CboGender.SelectedItem;
                    return brethren.BrethrenFull.Gender == gender;
                };
            }
        }

        private void CboLocalStatus_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboLocalStatus)){
                _brethrenView.Filter = b =>{
                    var brethren = (BrethrenBasic) b;
                    var localstatus = (LocalStatus) CboLocalStatus.SelectedValue;
                    return brethren.LocalStatus == localstatus;
                };
            }
        }

        private void CboAttendanceStatus_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboAttendanceStatus)){
                _brethrenView.Filter = b =>{
                    var brethren = b as BrethrenBasic;
                    if (brethren != null){
                        var attendanceStatus = _statusIdentifier.GetStatusForLast12Session(brethren.Id);
                        return brethren.LocalStatus == LocalStatus.Present_Here &&
                               attendanceStatus == (AttendanceStatus) CboAttendanceStatus.SelectedValue;
                    }
                    return false;
                };
            }
        }

        private void CboPicture_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboPicture)){
                var item = (ComboBoxItem) CboPicture.SelectedValue;
                if (item.Content.ToString() == "Has Picture")
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && _imageService.HasPicture(brethren.Id);
                    };
                else
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && _imageService.HasPicture(brethren.Id) == false;
                    };
            }
        }

        private void CboNoInfo_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (HasSelectedItem(CboNoInfo)){
                var item = (ComboBoxItem) CboNoInfo.SelectedValue;
                if (item.Content.ToString() == "No Church ID")
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && string.IsNullOrWhiteSpace(brethren.ChurchId);
                    };

                else if (item.Content.ToString() == "No Name")
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && string.IsNullOrWhiteSpace(brethren.Name);
                    };

                else if (item.Content.ToString() == "No Date of Baptism")
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && brethren.BrethrenFull.DateofBaptism == null;
                    };
                else if (item.Content.ToString() == "No Contact Info")
                    _brethrenView.Filter = b =>{
                        var brethren = b as BrethrenBasic;
                        return brethren != null && brethren.BrethrenFull.Contactno == null;
                    };
            }
        }

        private void AddPicture_Click(object sender, RoutedEventArgs e){
            var openFile = new OpenFileDialog{Filter = "JPEG Picture (*.JPG)|*.JPG"};
            var isOpen = openFile.ShowDialog();

            if (isOpen != true) return;
            var brethrenId = ((BrethrenBasic) _brethrenView.CurrentItem).Id;
            SetPicture(brethrenId, openFile.FileName);
        }

        private void SetPicture(long brethrenId, string fileName){
            _imageService.SaveImage(fileName, brethrenId);
            Image.Source = ImageToBitmap.ConvertToBitmapImage(_imageService.GetPicture(brethrenId));
            Resources["EnableAddPicture"] = _imageService.HasPicture() == false;
        }

        private void DeletePicture_Click(object sender, RoutedEventArgs e){
            if (_brethrenView.CurrentItem == null){
                MessageBox.Show("Please Select Brethren from the List");
                return;
            }
            var brethrenId = ((BrethrenBasic) _brethrenView.CurrentItem).Id;
            _imageService.RemovePicture(brethrenId);
            Image.Source = ImageToBitmap.ConvertToBitmapImage(_imageService.GetPicture(brethrenId));
            Resources["EnableAddPicture"] = _imageService.HasPicture() == false;
        }

        private void PrintInfoButton_Click(object sender, RoutedEventArgs e){
            var brethren = _brethrenView.CurrentItem as BrethrenBasic;
            if (brethren == null){
                MessageBox.Show("You must select brethren from the List", "Can't print", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            var picture = _imageService.GetPicture(brethren.Id);
            var brethrenData = new BrethrenDataForm(brethren, picture);
            brethrenData.ShowDialog();
        }
    }
}
