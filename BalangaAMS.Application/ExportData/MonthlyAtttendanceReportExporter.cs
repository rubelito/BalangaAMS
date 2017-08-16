using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using ClosedXML.Excel;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class MonthlyAtttendanceReportExporter :IExportMonthlyAttendanceReport
    {
        private const int RowColumnIndex = 5;
        private const int RowIndex = RowColumnIndex + 1;
        private MonthlyAttendanceGroupInfo _reportInfo;

        public void ExportMonthlyAttendanceReport(MonthlyAttendanceGroupInfo reportInfo)
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
                RemoveSeparatorColumn(reportTable);
                DisplayAttendanceLogo(attendanceSheet, reportTable.Columns.Count);
                DisplayDivisionAndMonth(attendanceSheet);
                DisplayDistrictName(attendanceSheet);
                DisplayGroupName(attendanceSheet);
                CreateColumnHeader(attendanceSheet, reportTable);
                CreateColumn(attendanceSheet, reportTable);
                CreateRow(attendanceSheet, reportTable);
                CreateConformed(attendanceSheet, reportTableRowsCount);
                CreateLedger(attendanceSheet, reportTableRowsCount);

                wb.SaveAs(destinationPath);
            }

        private void RemoveSeparatorColumn(DataTable reportTable)
            {
                if (reportTable.Columns.Contains("Separator"))
                    reportTable.Columns.Remove("Separator");
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
                monthCell.Value = "MONTH: " + _reportInfo.MonthofYear + " ," + _reportInfo.Year;
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

            private void CreateColumnHeader(IXLWorksheet attendanceSheet, DataTable reportTable)
            {
                var  columnList = new List<string>();
                foreach (DataColumn column in reportTable.Columns)
                {
                    columnList.Add(column.ColumnName);
                }
                CreateHeaderForPrayerMeeting(attendanceSheet, columnList);
                CreateHeaderForWorshipService(attendanceSheet, columnList);
                CreateHeaderForThanksGiving(attendanceSheet, columnList);
                CreateHeaderForTotal(attendanceSheet, columnList);

            }           

                private void CreateHeaderForPrayerMeeting(IXLWorksheet attendanceSheet, List<string> columnList)
                {
                    if (IsColumnListHasDtcol("dtcol_P_", columnList))
                    {
                        int indexOfFirstPrayerMeeting = GetIndexOfFirstDtcol("dtcol_P_", columnList);
                        int indexOfTotalPrayerMeeting = GetIndexOfFirstDtcol("dtcol_P_Total", columnList);
                        CreateHeader(attendanceSheet, "Prayer Meeting", indexOfFirstPrayerMeeting,
                            indexOfTotalPrayerMeeting);
                    }         
                }

                private void CreateHeaderForWorshipService(IXLWorksheet attendanceSheet, List<string> columnList)
                {
                    if (IsColumnListHasDtcol("dtcol_W_", columnList))
                    {
                        int indexOfFirstWorshipService = GetIndexOfFirstDtcol("dtcol_w_", columnList);
                        int indexOfTotalWorshipService = GetIndexOfFirstDtcol("dtcol_W_Total", columnList);
                        CreateHeader(attendanceSheet, "Worship Service", indexOfFirstWorshipService,
                            indexOfTotalWorshipService);
                    }
                }

                private void CreateHeaderForThanksGiving(IXLWorksheet attendanceSheet, List<string> columnList)
                {
                    if (IsColumnListHasDtcol("dtcol_W_", columnList))
                    {
                        int indexOfFirstThanksGiving = GetIndexOfFirstDtcol("dtcol_T_", columnList);
                        int indexOfTotalThanksGiving = GetIndexOfFirstDtcol("dtcol_T_Total", columnList);
                        CreateHeader(attendanceSheet, "Thanks Giving", indexOfFirstThanksGiving,
                            indexOfTotalThanksGiving);
                    }
                }

                private void CreateHeaderForTotal(IXLWorksheet attendanceSheet, List<string> columnList)
                {
                    CreateHeader(attendanceSheet, "Total", columnList.Count - 3, columnList.Count);
                }

                private int GetIndexOfFirstDtcol(string dtcol, List<string> columnList){
                    int indexOfFirstDtcol = 0;
                    for (int i = 0; i <= columnList.Count + 1; i++){
                        if (columnList[i].IndexOf(dtcol, StringComparison.OrdinalIgnoreCase) >= 0){
                            indexOfFirstDtcol = i + 1;
                            break;
                        }
                    }
                    return indexOfFirstDtcol;
                }

                private bool IsColumnListHasDtcol(string dtcol, List<string> columnList){
                    bool hasDtCol = false;
                    foreach (var columnName in columnList){
                        if (columnName.IndexOf(dtcol, StringComparison.OrdinalIgnoreCase) >= 0){
                            hasDtCol = true;
                            break;
                        }
                    }
                    return hasDtCol;
                }

                private void CreateHeader(IXLWorksheet worksheet, string headerName, int startIndex, int lastIndex){
                    var cellRange = worksheet.Range(RowColumnIndex - 1, startIndex, RowColumnIndex - 1, lastIndex).Merge();
                    cellRange.Value = headerName;
                    DrawColumnHeaderStyle(cellRange);
                }

        private void DrawColumnHeaderStyle(IXLRange cellRange)
                        {
                            cellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cellRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            cellRange.Style.Border.TopBorderColor = XLColor.Blue;
                            cellRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            cellRange.Style.Border.LeftBorderColor = XLColor.Blue;
                            cellRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            cellRange.Style.Border.RightBorderColor = XLColor.Blue;
                            cellRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            cellRange.Style.Border.BottomBorderColor = XLColor.Blue;
                        }

                private void CreateColumn(IXLWorksheet worksheet, DataTable reportTable)
                {
                    int columnIndex = 1;
                    foreach (DataColumn column in reportTable.Columns)
                    {
                        var columnName = column.ColumnName;
                        if (columnName.Contains("dtcol_"))
                            columnName = Removedtcol(columnName);
                        var cell = worksheet.Cell(RowColumnIndex, columnIndex);
                        cell.Value = columnName;
                        worksheet.Column(columnIndex).AdjustToContents();
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        DrawHeaderStyle(cell);
                        columnIndex++;
                    }
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

                    private string Removedtcol(string str)
                    {
                        string finalstr;
                        if (str.Contains("Total"))
                            finalstr = str.Substring(8, str.Length - 8);
                        else
                        {
                            str = str.Substring(8, str.Length - 8);
                            string[] splitDate = str.Split('Z');
                            var date = splitDate[0] + "/" + splitDate[1] + "/" + splitDate[2];
                            finalstr = removeUnderScore(date);
                            var dayOnly = Convert.ToDateTime(finalstr).Day;
                            finalstr = dayOnly.ToString(CultureInfo.InvariantCulture);
                        }
                        return finalstr;
                    }

                        private string removeUnderScore(string date)
                        {
                            int index = date.IndexOf("_", StringComparison.Ordinal);
                            return date.Substring(0, index);
                        }

                private void CreateRow(IXLWorksheet worksheet, DataTable reportTable)
                {
                    int rowIndex = RowIndex;
                    int columnCount = reportTable.Columns.Count;

                    foreach (DataRow row in reportTable.Rows)
                    {
                        CreateRowData(worksheet, row, rowIndex, columnCount);
                        rowIndex++;
                    }
                }

                    private void CreateRowData(IXLWorksheet worksheet, DataRow row, int rowIndex, int columnCount)
                    {
                        int columnIndex = 0;
                        for (int i = 0; i <= columnCount - 1; i++){
                            var cell = worksheet.Cell(rowIndex, columnIndex + 1);
                            cell.Value = ChangeValueToShorterValue(row[columnIndex].ToString());
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.DataType = XLCellValues.Text;
                            CreateBorderStyle(cell);
                            worksheet.Column(columnIndex + 1).AdjustToContents();
                            FillBackGroudColor(cell);
                            columnIndex++;
                        }
                    }

                        private string ChangeValueToShorterValue(string oldValue)
                        {
                            var newValue = oldValue;
                            if (oldValue == "Present")
                                newValue = "P";
                            else if (oldValue == "Late")
                                newValue = "L";
                            else if (oldValue == "Absent")
                                newValue = "X";
                            else if (oldValue == "Active")
                                newValue = "A";
                            else if (oldValue == "Inactive")
                                newValue = "I";
                            else if (oldValue == "NA")
                                newValue = "N/A";
                            else if (oldValue == "None")
                                newValue = " ";
                            return newValue;
                        }

                        private void FillBackGroudColor(IXLCell cell)
                        {
                            if ((string) cell.Value == "P" || (string) cell.Value == "X" ||
                                (string) cell.Value == "A" || (string) cell.Value == "I"){
                                cell.Style.Fill.BackgroundColor = (string) cell.Value == "P" ||
                                                                  (string) cell.Value == "A"
                                    ? XLColor.LightGreen
                                    : XLColor.LightPink;
                            }
                            else if ((string) cell.Value == "L")
                                cell.Style.Fill.BackgroundColor = XLColor.Yellow;
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
                DisplayPresentLedger(attendanceSheet, reportTableRowsCount);
                DisplayActiveLedger(attendanceSheet, reportTableRowsCount);
                DisplayLateLedger(attendanceSheet, reportTableRowsCount);
                DisplayAbsentLedger(attendanceSheet, reportTableRowsCount);
                DisplayInactiveLedger(attendanceSheet, reportTableRowsCount);
                DisplayNALedger(attendanceSheet, reportTableRowsCount);
            }

            private void DisplayPresentLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var presentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 10);
                presentCell.Value = "P";
                presentCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                presentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 4, 11);
                presentCellMeaning.Value = "= Present";
            }

            private void DisplayActiveLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var presentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 10);
                presentCell.Value = "A";
                presentCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                presentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 5, 11);
                presentCellMeaning.Value = "= Active";
            }

            private void DisplayLateLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var lateCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 6, 10);
                lateCell.Value = "L";
                lateCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                lateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 6, 11);
                presentCellMeaning.Value = "= Present but Late";
            }

            private void DisplayAbsentLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var absentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 10);
                absentCell.Value = "X";
                absentCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                absentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 7, 11);
                presentCellMeaning.Value = "= Absent";
            }

            private void DisplayInactiveLedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var absentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 8, 10);
                absentCell.Value = "I";
                absentCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                absentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 8, 11);
                presentCellMeaning.Value = "= Inactive";
            }

            private void DisplayNALedger(IXLWorksheet attendanceSheet, int reportTableRowsCount)
            {
                var absentCell = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 9, 10);
                absentCell.Value = "N/A";
                absentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var presentCellMeaning = attendanceSheet.Cell(reportTableRowsCount + RowColumnIndex + 9, 11);
                presentCellMeaning.Value = "= Newly Baptized/Transferred";
            }
    }
}
