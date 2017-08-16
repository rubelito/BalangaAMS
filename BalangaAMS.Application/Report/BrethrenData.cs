namespace BalangaAMS.ApplicationLayer.Report
{
    using System.Drawing;

    /// <summary>
    /// Summary description for BrethrenData.
    /// </summary>
    public partial class BrethrenData : Telerik.Reporting.Report
    {
        private readonly Image _picture;

        public BrethrenData(Image picture)
        {
            _picture = picture;
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            pictureBox1.Value = _picture;
        }
    }
}