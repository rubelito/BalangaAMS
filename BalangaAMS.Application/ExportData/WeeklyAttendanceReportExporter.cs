using System;
using System.Data;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain.Enum;
using ClosedXML.Excel;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class WeeklyAttendanceReportExporter : IExportWeeklyAttendanceReport
    {
        private const int RowColumnIndex = 5;
        private const int RowIndex = RowColumnIndex + 1;
        private WeeklyAttendanceGroupInfo _reportInfo;

        public void ExportWeeklyAttendanceReport(WeeklyAttendanceGroupInfo reportInfo)
        {
            _reportInfo = reportInfo;
            if (IsBrethrenEmpty(_reportInfo.ReportTable))
                throw new ArgumentNullException("Report Table List should not be empty");
            if (FileNameChecker.IsNotValidPathOrFileName(_reportInfo.DestinationPath))
                throw new Exception("Invalid File Path");

            ExportReport(_reportInfo.ReportTable, _reportInfo.DestinationPath, _reportInfo.GroupName);
        }

            private bool IsBrethrenEmpty(DataTable reportTableList)
            {
                bool isEmpty = reportTableList == null || reportTableList.Rows.Count == 0;
                return isEmpty;
            }

            private void ExportReport(DataTable reportTable, string destinationPath, string groupName)
            {
                var reportTableRowsCount = reportTable.Rows.Count;
                var wb = new XLWorkbook();
                var attendanceSheet = wb.Worksheets.Add(groupName);
                DisplayAttendanceLogo(attendanceSheet, reportTable.Columns.Count);
                DisplayDivisionAndMonth(attendanceSheet);
                DisplayDistrictName(attendanceSheet);
                DisplayGroupName(attendanceSheet);
                CreateColumn(attendanceSheet, reportTable);
                CreateRow(attendanceSheet, reportTable);
                CreateConformed(attendanceSheet, reportTableRowsCount);
                CreateLedger(attendanceSheet, reportTableRowsCount);

                wb.SaveAs(destinationPath);
            }      

            private void DisplayAttendanceLogo(IXLWorksheet worksheet, int columnCount)
            {
                var cell = worksheet.Range(1, 1, 1, columnCount).Merge();
                cell.Value = "Attendance Monitoring System";
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 20;
            }

            private void DisplayDivisionAndMonth(IXLWorksheet worksheet)
            {
                var divisionCell = worksheet.Range(2, 1, 2, 2).Merge();
                var monthCell = worksheet.Range(2, 3, 2, 13).Merge();
                divisionCell.Value = _reportInfo.DivisionName + " DIVISION";
                divisionCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                monthCell.Value = "DATE: " + _reportInfo.DateCoverage;
                monthCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                monthCell.Style.Font.Bold = true;
            }

            private void DisplayDistrictName(IXLWorksheet worksheet)
            {
                var districtCell = worksheet.Range(RowColumnIndex - 2, 1, RowColumnIndex - 2, 2).Merge();
                districtCell.Value = _reportInfo.DistrictName;
                districtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }

            private void DisplayGroupName(IXLWorksheet worksheet)
            {
                var groupCell = worksheet.Range(RowColumnIndex - 1, 1, RowColumnIndex - 1, 2).Merge();
                groupCell.Value = "GROUP: " + _reportInfo.GroupName;
                groupCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                groupCell.Style.Font.FontSize = 19;
                groupCell.Style.Font.Bold = true;
            }

        private void CreateColumn(IXLWorksheet worksheet, DataTable reportTable){
            int columnIndex = 1;
            foreach (DataColumn column in reportTable.Columns){
                var columnName = column.ColumnName;
                if (columnName.Contains("Z")){
                    var gatheringName = RemoveExactDateOnColumnName(columnName);
                    CreateGatheringTitleColumn(worksheet, gatheringName, columnIndex);
                    columnName = GetDateOnColumnName(columnName);
                    columnName = RemoveLastUnderScore(columnName);
                }
                if (columnName.Contains("REMARKS"))
                    columnName = RemoveUnderScore(columnName);
                var cell = worksheet.Cell(RowColumnIndex, columnIndex);
                cell.Value = columnName;
                worksheet.Column(columnIndex).AdjustToContents();
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                DrawHeaderStyle(cell);
                columnIndex++;
            }
        }

        private void CreateGatheringTitleColumn(IXLWorksheet worksheet, string columnName, int columnIndex){
            var titleCell =
                worksheet.Range(RowColumnIndex - 1, columnIndex, RowColumnIndex - 1, columnIndex + 1).Merge();
            titleCell.Value = columnName;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private string RemoveExactDateOnColumnName(string columnName){
            int ZIndex = columnName.IndexOf("Z");
            return columnName.Substring(0, ZIndex);
        }

        private string GetDateOnColumnName(string columnName){
            int ZIndex = columnName.IndexOf("Z");
            int dateCount = columnName.Length - ZIndex;
            var dateStr = columnName.Substring(ZIndex, dateCount);
            return dateStr.Replace("Z", " ");
        }

        private string RemoveUnderScore(string columnName){
                int underScoreIndex = columnName.IndexOf("_");
                return columnName.Substring(0, underScoreIndex);
            }

        private string RemoveLastUnderScore(string columnName){
            var underScoreIndex = columnName.LastIndexOf("_");
            return columnName.Substring(0, underScoreIndex);
        }

            private void DrawHeaderStyle(IXLCell cell)
            {
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.TopBorderColor = XLColor.LightBlue;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.LeftBorderColor = XLColor.LightBlue;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.RightBorderColor = XLColor.LightBlue;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.BottomBorderColor = XLColor.LightBlue;
            }

            private void CreateRow(IXLWorksheet worksheet, DataTable reportTable)
            {
                int rowIndex = RowIndex;
                int columnCount = reportTable.Columns.Count;

                foreach (DataRow row in reportTable.Rows){
                    CreateRowData(worksheet, row, rowIndex, columnCount);
                    rowIndex++;
                }
            }

            private void CreateRowData(IXLWorksheet worksheet, DataRow row, int rowIndex, int columnCount)
            {
                int columnIndex = 0;              
                for (int i = 0; i <= columnCount - 1; i++) {
                    var cell = worksheet.Cell(rowIndex, columnIndex + 1);
                    if (IsValueIsDayInfoObject(row[columnIndex]))
                        ChangeValueToShorterValue(row[columnIndex], cell);
                    else {
                        cell.Value = row[columnIndex].ToString();
                        cell.DataType = XLCellValues.Text;
                    }
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    CreateBorderStyle(cell);
                    worksheet.Column(columnIndex + 1).AdjustToContents();
                    columnIndex++;
                }
            }

        private void CreateBorderStyle(IXLCell cell){
            cell.Style.Border.TopBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.TopBorderColor = XLColor.LightBlue;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.LeftBorderColor = XLColor.LightBlue;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.RightBorderColor = XLColor.LightBlue;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.BottomBorderColor = XLColor.LightBlue;           
        }

        private void ChangeValueToShorterValue(object oldValue, IXLCell cell){
            if (IsValueIsDayInfoObject(oldValue)){
                var dayInfo = oldValue as AttendanceDayInfo;
                if (dayInfo.DayAttendanceStatus != DayAttendanceStatus.None){
                    switch (dayInfo.DayAttendanceStatus){
                        case DayAttendanceStatus.Present:
                            cell.Value = DayAttendanceStatus.Present;
                            FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                            break;
                        case DayAttendanceStatus.Late:
                            cell.Value = DayAttendanceStatus.Late;
                            FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                            break;
                        case DayAttendanceStatus.Absent:
                            cell.Value = DayAttendanceStatus.Absent;
                            FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                            break;
                        case DayAttendanceStatus.NA:
                            cell.Value = "N/A";
                            break;
                    }
                }
            }
        }

        private bool IsValueIsDayInfoObject(object oldValue) 
            {
                var dayInfo = oldValue as AttendanceDayInfo;
                return dayInfo != null;
            }

            private void FillBackGroudColor(IXLCell cell, DayAttendanceStatus dayInfo)
            {
                if (dayInfo == DayAttendanceStatus.Present || dayInfo == DayAttendanceStatus.Absent)
                {
                    cell.Style.Fill.BackgroundColor = dayInfo == DayAttendanceStatus.Present
                        ? XLColor.LightGreen
                        : XLColor.LightPink;
                }
                else if (dayInfo == DayAttendanceStatus.Late)
                    cell.Style.Fill.BackgroundColor = XLColor.Yellow;
            }

            private void CreateConformed(IXLWorksheet worksheet, int reportTableRowsCount)
            {
                var conformedCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 1);
                conformedCell.Value = "CONFORMED:";
                var deaconLineCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 2);
                deaconLineCell.Value = "________________";
                var deaconNameCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 2);
                deaconNameCell.Value = "DEACON";
                var secretaryLineCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 2);
                secretaryLineCell.Value = "________________";
                var secretaryNameCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 8, 2);
                secretaryNameCell.Value = "SECRETARY";
                var deaconessLineCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 3);
                deaconessLineCell.Value = "________________";
                var deaconessCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 3);
                deaconessCell.Value = "DEACONESS";
                var assignedWorkerLineCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 3);
                assignedWorkerLineCell.Value = "________________";
                var assignedWorkerCell = worksheet.Cell(reportTableRowsCount + RowColumnIndex + 8, 3);
                assignedWorkerCell.Value = "ASSIGNED WORKER & ID NUMBER";
            }

            private void CreateLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                DisplayPresent(attendanceSheet, reportTableRowsCount);
                DisplayLate(attendanceSheet, reportTableRowsCount);
                DisplayAbsent(attendanceSheet, reportTableRowsCount);
                DisplayNa(attendanceSheet, reportTableRowsCount);
            }

        private void DisplayPresent(IXLWorksheet attendanceSheet, int reportTableRowsCount)
        {
            var presentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 4);
            presentCell.Value = "Present";
            presentCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
            presentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 5);
            presentCellMeaning.Value = "= Present";
        }

        private void DisplayLate(IXLWorksheet attendanceSheet, int reportTableRowsCount)
        {
            var lateCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 4);
            lateCell.Value = "Late";
            lateCell.Style.Fill.BackgroundColor = XLColor.Yellow;
            lateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 5);
            presentCellMeaning.Value = "= Late";
        }

        private void DisplayAbsent(IXLWorksheet attendanceSheet, int reportTableRowsCount)
        {
            var absentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 6, 4);
            absentCell.Value = "Absent";
            absentCell.Style.Fill.BackgroundColor = XLColor.LightPink;
            absentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 6, 5);
            presentCellMeaning.Value = "= Absent";
        }

        private void DisplayNa(IXLWorksheet attendanceSheet, int reportTableRowsCount)
        {
            var absentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 4);
            absentCell.Value = "N/A";
            absentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 5);
            presentCellMeaning.Value = "= Newly Baptized/Transferred";
        }
    }
}
