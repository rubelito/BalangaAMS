using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.VisualBasic;

namespace BalangaAMS.WPF.View
{
    public partial class ManageGroupings
    {
        private ObservableCollection<Group> _groups; 
        private ObservableCollection<BrethrenBasic> _brethrenNoGroup;
        private ICollectionView _brethrenNoGroupview;
        private ICollectionView _groupview;
        private IGroupManager _groupManager;
        
        public ManageGroupings()
        {
            InitializeComponent();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            InitializeData();
        }

        private void InitializeData()
        {
            _groups = new ObservableCollection<Group>(_groupManager.Getallgroup());
            _brethrenNoGroup = new ObservableCollection<BrethrenBasic>(_groupManager.GetBrethrenWithNoGroup());

            _groupview = CollectionViewSource.GetDefaultView(_groups);
            WithGroup.DataContext = _groupview;
            TabItemEditGroup.DataContext = _groupview;
            listwithgroup.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            
            _brethrenNoGroupview = CollectionViewSource.GetDefaultView(_brethrenNoGroup);
            _brethrenNoGroupview.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            WithNoGroup.DataContext = _brethrenNoGroupview;
        }

        private void Remove_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedBrethren = listwithgroup.SelectedItems.Cast<BrethrenBasic>();

            foreach (var brethren in selectedBrethren.ToArray()){
                RemoveBrethrenToAGroup(brethren);
            }
        }

        private void RemoveBrethrenToAGroup(BrethrenBasic brethren){
            _groupManager.RemoveBrethrenToAGroup(brethren.Id);
            _brethrenNoGroup.Add(brethren);
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            var selectedBrethren = Listwithnogroup.SelectedItems.Cast<BrethrenBasic>();
            foreach (var brethren in selectedBrethren.ToArray())
            {
                AssignBrethrenToAGroup(brethren);
            }
        }

        private void AssignBrethrenToAGroup(BrethrenBasic brethren){
            var group = (Group) cbowithgroup.SelectedItem;
            _groupManager.AddBrethrenToAGroup(brethren.Id, @group.Id);
            _brethrenNoGroup.Remove(brethren);
        }

        private void seach_TextChanged(object sender, TextChangedEventArgs e)
        {
            _brethrenNoGroupview.Filter = BrethrenFilter;
        }

        private bool BrethrenFilter(object item)
        {
            var brethren = item as BrethrenBasic;
            return brethren != null && 
                !string.IsNullOrWhiteSpace(brethren.Name) &&
                brethren.Name.IndexOf(seach.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var group = ListBoxgroup.SelectedItem as Group;
            if (group == null)
            {
                MessageBox.Show("You must select Group from the List", "Can't Edit", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            var oldgroupname = group.GroupName;
            var newgroupname = Interaction.InputBox("Edit Group Name", "Edit Group", oldgroupname);
            if (IsOldGroupNameIsNotSimilarWithNewName(oldgroupname, newgroupname) &&
                IsNewGroupNameIsnotEmpty(newgroupname)){
                group.GroupName = newgroupname;
                _groupManager.Updategroup(group);
            }
        }

        private static bool IsNewGroupNameIsnotEmpty(string newgroupname){
            return !string.IsNullOrWhiteSpace(newgroupname);
        }

        private static bool IsOldGroupNameIsNotSimilarWithNewName(string oldgroupname, string newgroupname){
            return oldgroupname != newgroupname;
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            var group = ListBoxgroup.SelectedItem as Group;

            MessageBoxResult removeresult = MessageBox.Show("Do you want to remove this group?", "Removing Group"
                                                        , MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (removeresult == MessageBoxResult.Yes)
            {                
                _groups.Remove(group);
                _groupManager.Removegroup(group);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e){
            var groupname = Interaction.InputBox("Add Another Group Name", "Add Group");
            if (groupname.Length != 0){
                AddNewGroup(groupname);
            }
        }

        private void AddNewGroup(string groupname){
            var newgroup = new Group{GroupName = groupname};
            _groupManager.Addgroup(newgroup);
            _groups.Add(newgroup);
        }
    }
}
