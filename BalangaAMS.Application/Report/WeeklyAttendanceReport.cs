using System;
using System.Data;
using System.Drawing;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.Core.Domain.Enum;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace BalangaAMS.ApplicationLayer.Report
{
    /// <summary>
    /// Summary description for WeeklyAttendanceReport.
    /// </summary>
    public partial class WeeklyAttendanceReport : Telerik.Reporting.Report
    {
        private readonly DataTable _reportTable;
        private readonly WeeklyReportSummary _weeklyReport;
        TableGroup _columngroup;
        TextBox _txtcolumn;
        TextBox _daybody;
        TextBox _txtbody;

        public WeeklyAttendanceReport(DataTable reportTable, WeeklyReportSummary weeklyReport, string localName)
        {
            _reportTable = reportTable;
            _weeklyReport = weeklyReport;
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            TitleTextbox.Value = localName + " Attendance Monitoring System";
            //
            // TODO: Add any constructor code after InitializeComponent cal
            //
        }

        private void WeeklyAttendanceReport_NeedDataSource(object sender, EventArgs e)
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

                if (IsValueIsDayInfoObject(columns[i]))
                {
                    CreateDayInfoField(columns[i], i);
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
            FillSummaryReport();
        }

        private bool IsValueIsDayInfoObject(DataColumn column)
        {
            return column.DataType == typeof(AttendanceDayInfo);
        }

        private void CreateColumn(DataColumn column)
        {
            string columnName = column.ColumnName;
            _txtcolumn = new TextBox {Size = new SizeU(Unit.Inch(1), Unit.Inch(0.2))};
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

            else if (columnName.Contains("Z"))
            {
                columnName = RemoveExactDateOnColumnName(columnName);
            }
            _txtcolumn.Value = columnName;
        }

        private void CreateDayInfoField(DataColumn column, int columnPosition)
        {
            _daybody = new TextBox {Value = "=Fields." + column.ColumnName};
            _daybody.ItemDataBinding += TemporaryTextboxOnItemDataBinding;
            _daybody.Style.BorderColor.Default = Color.Blue;
            _daybody.Style.BorderStyle.Default = BorderType.Solid;
            _daybody.Size = new SizeU(Unit.Inch(1), Unit.Inch(1));
            FormatTextBoxValue(_daybody);
            table1.Body.SetCellContent(0, columnPosition, _daybody);
            table1.Items.AddRange(new ReportItemBase[] { _daybody, _txtcolumn });
        }

        private void TemporaryTextboxOnItemDataBinding(object sender, EventArgs eventArgs)
        {
            var txt = (Telerik.Reporting.Processing.TextBox)sender;
            var columnName = RemoveFieldTextInValue(txt.Text);
            var dataObject = txt.DataObject;
            var dayInfo = ((AttendanceDayInfo) dataObject[columnName]);
            switch (dayInfo.DayAttendanceStatus)
            {
                case DayAttendanceStatus.Present:
                    txt.Value = dayInfo.DayAttendanceStatus;
                    txt.Style.BackgroundColor = Color.LightGreen;
                    break;
                case DayAttendanceStatus.Late:
                    txt.Value = dayInfo.DayAttendanceStatus;
                    txt.Style.BackgroundColor = Color.Yellow;
                    break;
                case DayAttendanceStatus.Absent:
                    txt.Value = dayInfo.DayAttendanceStatus;
                    txt.Style.BackgroundColor = Color.LightPink;
                    break;
                case DayAttendanceStatus.NA:
                    txt.Value = "N/A";
                    break;
                case DayAttendanceStatus.None:
                    txt.Value = "-";
                    break;
            }
        }

        private void CreateTextField(DataColumn column, int columnPosition)
        {
            _txtbody = new TextBox {Value = "=Fields." + column.ColumnName};
            _txtbody.Style.BorderColor.Default = Color.Black;
            _txtbody.Style.BorderStyle.Default = BorderType.Solid;
            
            if (column.ColumnName == "Name")
            {
                _txtbody.Size = new SizeU(Unit.Inch(1.2), Unit.Inch(1));
            }
            FormatTextBoxValue(_txtbody);

            table1.Body.SetCellContent(0, columnPosition, _txtbody);
            table1.Items.AddRange(new ReportItemBase[] { _txtbody, _txtcolumn });
        }

        private void FormatTextBoxValue(TextBox textBox)
        {
            textBox.Style.TextAlign = HorizontalAlign.Center;         
            textBox.Multiline = true;
            textBox.TextWrap = true;
            textBox.CanGrow = true;
            textBox.CanShrink = true;
        }

        private string RemoveExactDateOnColumnName(string columnName)
        {
            int ZIndex = columnName.IndexOf("Z");
            return columnName.Substring(0, ZIndex);
        }

        private string RemoveFieldTextInValue(string value)
        {
            int dotIndex = value.IndexOf(".") + 1;
            return value.Substring(dotIndex, value.Length - dotIndex);
        }

        private void FillSummaryReport()
        {
            txtGroup.Value = _weeklyReport.GroupName;
            txtDateCoverage.Value = _weeklyReport.DateCoverage;
        }
    }
}