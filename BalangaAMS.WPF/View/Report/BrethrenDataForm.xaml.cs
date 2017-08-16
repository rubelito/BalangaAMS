using System.Windows;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.Core.Domain;
using Telerik.Reporting;
using Image = System.Drawing.Image;

namespace BalangaAMS.WPF.View.Report
{
    /// <summary>
    /// Interaction logic for BrethrenDataForm.xaml
    /// </summary>
    public partial class BrethrenDataForm
    {
        private readonly BrethrenBasic _brethren;
        private readonly Image _picture;

        public BrethrenDataForm(BrethrenBasic brethren, Image picture)
        {
            _brethren = brethren;
            _picture = picture;
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var report = new BrethrenData(_picture) {DataSource = _brethren};
            var reportSource = new InstanceReportSource { ReportDocument = report };
            ReportViewer.ReportSource = reportSource;
            ReportViewer.RefreshReport();
        }
    }
}
