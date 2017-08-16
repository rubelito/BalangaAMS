using System;
using System.Data;
using System.Drawing;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace BalangaAMS.ApplicationLayer.Report
{
    /// <summary>
    /// Summary description for MonthyAttendanceReport.
    /// </summary>
    public partial class MonthlyAttendanceReport : Telerik.Reporting.Report
    {
        private readonly DataTable _reportTable;
        private readonly MontlyReportSummary _reportSummary;
        TableGroup _columngroup;
        TextBox _txtcolumn;
        PictureBox _picbody;
        TextBox _txtbody;

        public MonthlyAttendanceReport(DataTable reportTable, MontlyReportSummary reportSummary, string localName)
        {
            _reportTable = reportTable;
            _reportSummary = reportSummary;
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            TitleTextBox.Value = localName + " Attendance Monitoring System";
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void MonthyAttendanceReport_NeedDataSource(object sender, EventArgs e)
        {
            var columns = _reportTable.Columns;
            table1.DataSource = _reportTable;

            table1.ColumnGroups.Clear();
            table1.Body.Columns.Clear();
            table1.Body.Rows.Clear();
            table1.RowGroups.Clear();
       
            table1.Body.Rows.Add(new TableBodyRow());

            for (int i = 0; i <= columns.Count - 1; i++)
            {
                var tableColumn = new TableBodyColumn();
                table1.Body.Columns.Add(tableColumn);
                _columngroup = new TableGroup();
                _columngroup.Name = columns[i].ColumnName;
                table1.ColumnGroups.Add(_columngroup);
                
                CreateColumn(columns[i]);

                if (isImage(columns[i]))
                {
                    CreateImageField(columns[i], i);
                }
                else
                {
                    CreateTextField(columns[i], i);
                }
            }
            table1.ColumnHeadersPrintOnEveryPage = true;
            var tableBodyGroup = new TableGroup();
            tableBodyGroup.Groupings.Add(new Grouping(null));
            tableBodyGroup.Name = "Detail";
            table1.RowGroups.Add(tableBodyGroup);
            FillSummary();
        }

        private bool isImage(DataColumn column)
        {
            return column.DataType == typeof(byte[]);
        }

        private void CreateColumn(DataColumn column)
        {
            string columnName = column.ColumnName;
            _txtcolumn = new TextBox();
            _txtcolumn.Size = new SizeU(Unit.Inch(0.45), Unit.Inch(0.2));
            _txtcolumn.Style.Font.Size = new Unit("7PT");
            _txtcolumn.Style.BorderColor.Default = Color.Black;
            _txtcolumn.Style.BorderStyle.Default = BorderType.Solid;
            _txtcolumn.Style.TextAlign = HorizontalAlign.Center;
            _columngroup.ReportItem = _txtcolumn;

            if (columnName == "ChurchId")
            {
                _txtcolumn.Size = new SizeU(Unit.Inch(0.8), Unit.Inch(0.1));
            }
      
            else if (columnName == "Name")
            {
                _txtcolumn.Size = new SizeU(Unit.Inch(1), Unit.Inch(0.1));
            }
            else if (columnName.Contains("dtcol_"))
            {
                _txtcolumn.Size = new SizeU(Unit.Inch(0.45), Unit.Inch(0.1));
                _txtcolumn.Style.BorderColor.Default = Color.Blue;
                _txtcolumn.Style.BorderStyle.Default = BorderType.Solid;
                columnName = Removedtcol(column.ColumnName);
                if (columnName == "Total")
                {
                    _txtcolumn.Style.BorderColor.Default = Color.Black;
                    _txtcolumn.Size = new SizeU(Unit.Inch(0.3), Unit.Inch(0.1));
                }
            }
            else if (columnName.Contains("Separator"))
            {
                columnName = "-";
                _txtcolumn.Size = new SizeU(Unit.Inch(0.10), Unit.Inch(0.1));
            }
            _txtcolumn.Value = columnName;
        }

        private void CreateImageField(DataColumn column, int columnPosition)
        {           
            _picbody = new PictureBox();
            _picbody.Value = "=Fields." + column.ColumnName;
            _picbody.Style.BorderColor.Default = Color.Blue;
            _picbody.Style.BorderStyle.Default = BorderType.Solid;
            _picbody.Sizing = ImageSizeMode.Center;
            table1.Body.SetCellContent(0, columnPosition, _picbody);
            table1.Items.AddRange(new ReportItemBase[] { _picbody, _txtcolumn });
        }

        private void CreateTextField(DataColumn column, int columnPosition)
        {           
            _txtbody = new TextBox();
            _txtbody.Value = "=Fields." + column.ColumnName;
            _txtbody.Style.BorderColor.Default = Color.Black;
            _txtbody.Style.BorderStyle.Default = BorderType.Solid;
            _txtbody.Style.TextAlign = HorizontalAlign.Center;
            if (column.ColumnName == "Name")
            {
                _txtbody.Size = new SizeU(Unit.Inch(1.2), Unit.Inch(1)); 
            }
            _txtbody.Multiline = true;
            _txtbody.TextWrap = true;
            _txtbody.CanGrow = true;
            _txtbody.CanShrink = true;

            table1.Body.SetCellContent(0, columnPosition, _txtbody);
            table1.Items.AddRange(new ReportItemBase[] { _txtbody, _txtcolumn });          
        }

        private void FillSummary()
        {
            txtgroup.Value = _reportSummary.GroupName;
            txtactive.Value = Convert.ToString(_reportSummary.ActiveCount);
            txtinactive.Value = Convert.ToString(_reportSummary.InactiveCount);
            txtmonth.Value = Convert.ToString(_reportSummary.MonthofYear);
            txtgatheringTotal.Value = Convert.ToString(_reportSummary.GatheringsTotal);
        }

        private string Removedtcol(string str)
        {
            string finalstr;
            if (str.Contains("Total"))
            {
                finalstr = str.Substring(8, str.Length - 8);
            }
            else
            {
                str = str.Substring(8, str.Length - 8);
                string[] splitDate = str.Split('Z');
                var date = splitDate[0] + "/" + splitDate[1] + "/" + splitDate[2];
                finalstr = removeUnderScore(date);
            }
            return finalstr;
        }

        private string removeUnderScore(string date)
        {
            int index = date.IndexOf("_");
            return date.Substring(0, index);
        }

    }
}