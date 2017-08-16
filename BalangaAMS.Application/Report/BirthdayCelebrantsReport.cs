namespace BalangaAMS.ApplicationLayer.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BirthdayCelebrantsReport.
    /// </summary>
    public partial class BirthdayCelebrantsReport : Telerik.Reporting.Report
    {
        private readonly string _month;

        public BirthdayCelebrantsReport(string month)
        {
            _month = month;
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            txtMonth.Value = _month;
        }
    }
}