using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using Telerik.Reporting;
using Group = BalangaAMS.Core.Domain.Group;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for ReportGroup.xaml
    /// </summary>
    public partial class ReportGroup
    {
        private readonly IGroupManager _groupManager;
        private readonly IBrethrenManager _brethrenManager;
        private readonly int _daysToConsiderNewlyBaptised;

        public ReportGroup()
        {
            InitializeComponent();
            _groupManager = UnityBootstrapper.Container.Resolve<IGroupManager>();
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _daysToConsiderNewlyBaptised =
                Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            FillComboBoxWithGroupNames();
        }

        private void FillComboBoxWithGroupNames()
        {
            var groups = _groupManager.Getallgroup();
            groups.Add(new Group {GroupName = "Newly Baptised"});
            groups.Add(new Group {GroupName = "No Group"});
            CboGroup.DataContext = groups;
        }

        private void CboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<BrethrenBasic> brethren;
            var group = CboGroup.SelectedItem as Group;
            if (group == null)
                return;
            if (group.GroupName == "Newly Baptised" || group.GroupName == "No Group")
            {
                brethren = GetBrethrenWithNoGroupOrNewlyBaptised(group);
            }
            else
            {
                brethren = _groupManager.GetBrethrenWithInGroup(group.Id);
            }

            DisplayReport(brethren, group);
        }

            private List<BrethrenBasic> GetBrethrenWithNoGroupOrNewlyBaptised(Group group)
            {
                var brethren = new List<BrethrenBasic>();
                if (group.GroupName == "Newly Baptised")
                {
                    var brethrenNoGroup = _groupManager.GetBrethrenWithNoGroup();
                    brethren = brethrenNoGroup.Where(b => b.BrethrenFull.DateofBaptism.HasValue &&
                                _brethrenManager.IsNewlyBaptised(b, _daysToConsiderNewlyBaptised, DateTime.Now))
                            .ToList();
                }
                else if (group.GroupName == "No Group")
                    brethren = _groupManager.GetBrethrenWithNoGroup();

                return brethren;
            }

            private void DisplayReport(List<BrethrenBasic> brethrenList, Group group)
            {
                if (brethrenList.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot generate report, no brethren in this group", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            
                var report = new GroupReport(group.GroupName) { DataSource = brethrenList };
                var reportSource = new InstanceReportSource {ReportDocument = report};
                ReportViewer1.ReportSource = reportSource;
                ReportViewer1.RefreshReport();
            }
    }
}
