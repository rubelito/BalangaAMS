using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using Telerik.Reporting;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for BirthDayCelebrantsReport.xaml
    /// </summary>
    public partial class ReportBirthDayCelebrants
    {
        private readonly IBrethrenManager _brethrenManager;

        public ReportBirthDayCelebrants()
        {
            _brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            InitializeComponent();
        }

        private void CboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var month = (int)(MonthofYear)CboMonth.SelectedItem;
            var brethren = _brethrenManager.FindBrethren(b => b.BrethrenFull.DateofBaptism.HasValue &&
                         b.BrethrenFull.DateofBaptism.Value.Month == month &&
                        b.LocalStatus == LocalStatus.Present_Here);
                                            
            if (brethren.Count == 0)
                MessageBox.Show("Cannot generate report, no birthday celebrants in this month", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                GenerateReport(brethren);
        }

        private void GenerateReport(List<BrethrenBasic> brethrenList)
        {
            var objectDataSource = new ObjectDataSource();
            var month = (MonthofYear) CboMonth.SelectedItem;
            objectDataSource.DataSource = GetBrethrenInfo(brethrenList);

            var report = new BirthdayCelebrantsReport(month.ToString()) { DataSource = objectDataSource };
            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = report;
            ReportViewer1.ReportSource = reportSource;
            ReportViewer1.RefreshReport();
        }

        private List<BrethrenInfo> GetBrethrenInfo(List<BrethrenBasic> brethrenList)
        {
            var brethrenInfo = new List<BrethrenInfo>();
            foreach (var b in brethrenList){
                brethrenInfo.Add(CreateBrethrenInfo(b));
            }
            return brethrenInfo.OrderBy(b => b.Name).ToList();
        }

        private BrethrenInfo CreateBrethrenInfo(BrethrenBasic brethren){
            return new BrethrenInfo{
                ChurchID = brethren.ChurchId,
                Name = brethren.Name,
                DateOfBaptism =
                    brethren.BrethrenFull.DateofBaptism.HasValue
                        ? brethren.BrethrenFull.DateofBaptism.Value.ToString("MMM dd, yyyy")
                        : " ",
                Age = Convert.ToString(ComputeAge(brethren.BrethrenFull.DateofBaptism))
            };
        }


        private int ComputeAge(DateTime? birthDate){
            if (birthDate == null)
                return 0;
            int yearsPassed = DateTime.Now.Year - birthDate.Value.Year;
            if (DateTime.Now.Month < birthDate.Value.Month || (DateTime.Now.Month == birthDate.Value.Month && DateTime.Now.Day < birthDate.Value.Day))
            {
                yearsPassed--;
            }
            return yearsPassed;
        }
    }
}
