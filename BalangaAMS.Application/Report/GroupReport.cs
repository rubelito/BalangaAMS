namespace BalangaAMS.ApplicationLayer.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for GroupReport.
    /// </summary>
    public partial class GroupReport : Telerik.Reporting.Report
    {
        private readonly string _groupName;

        public GroupReport(string groupName)
        {
            _groupName = groupName;
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            txtGroup.Value = _groupName;
        }
    }
}