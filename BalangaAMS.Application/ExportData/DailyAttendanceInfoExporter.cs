using System;
using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class DailyAttendanceInfoExporter : IExportDailyAttendanceInfo
    {
        private readonly int initialRowIndex =6;
        private readonly int initialColumnIndex=5;
        private GatheringSession _session;

        public void ExportDailyAttendanceInfo(List<AttendanceInfoDTO> attendanceDTOList, GatheringSession session
            ,string title ,string destinationPath){
            if (IsAttendanceDTOEmpty(attendanceDTOList))
                throw new ArgumentNullException("AttendanceInfoDTO should not be null");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid File Path");
            if (session == null)
                throw new Exception("Gathering should not be null");

            _session = session;
            Exporter(attendanceDTOList, title, destinationPath);
        }

        private bool IsAttendanceDTOEmpty(List<AttendanceInfoDTO> attendanceDTOList){
            return attendanceDTOList == null;
        }

        private void Exporter(List<AttendanceInfoDTO> attendanceDTOList, string title, string destinationPath){
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Daily Attendance Info");

            CreateHeader(ws, title);
            CreateRows(attendanceDTOList, ws);
            wb.SaveAs(destinationPath);
        }

        private void CreateHeader(IXLWorksheet ws, string title){
            var titleColumn = ws.Range(1, 1, 1, 3).Merge();
            titleColumn.Value = title;
            var gatheringCell = ws.Range(2, 1, 2, 3).Merge();
            gatheringCell.Value = "Gathering: " + _session.Gatherings + " - " + _session.Date.ToString("MMM dd, yyyy");

            var numberColumn = ws.Cell(initialColumnIndex, 1);
            CreateColumnCell(numberColumn, ws.Column(1),"NO.");

            var churchIdColumn = ws.Cell(initialColumnIndex, 2);
            CreateColumnCell(churchIdColumn, ws.Column(2), "Church ID");

            var nameColumn = ws.Cell(initialColumnIndex, 3);
            CreateColumnCell(nameColumn, ws.Column(3), "Name");

            var groupNameColumn = ws.Cell(initialColumnIndex, 4);
            CreateColumnCell(groupNameColumn, ws.Column(4), "Group");

            var logDateColumn = ws.Cell(initialColumnIndex, 5);
            CreateColumnCell(logDateColumn, ws.Column(5), "Log Date/Time");
        }

        private void CreateColumnCell(IXLCell cell , IXLColumn column, string value){
            cell.Value = value;
            DrawHeaderStyle(cell);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            column.AdjustToContents();
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

        private void CreateRows(List<AttendanceInfoDTO> attendanceDTOList, IXLWorksheet ws){
            int dataCount = 0;
            int rowIndex = initialRowIndex;
            foreach (var infoDTO in attendanceDTOList){
                dataCount++;
                CreateNumberRecords(ws, dataCount, rowIndex);
                CreateChurchIdCell(infoDTO.Brethren.ChurchId, ws, rowIndex);
                CreateNameCell(infoDTO.Brethren.Name, ws, rowIndex);
                CreateGroupNameCell(infoDTO.GroupName, ws, rowIndex);
                CreateLogDateCell(infoDTO.AttendaceDayInfo, ws, rowIndex);
                rowIndex++;
            }    
        }

        private void CreateNumberRecords(IXLWorksheet ws, int dataCount, int rowIndex){
            var numberCell = ws.Cell(rowIndex, 1);
            numberCell.Value = dataCount;
            CreateBorderStyle(numberCell);
        }

        private void CreateChurchIdCell(string churchId, IXLWorksheet ws, int rowIndex){
            var churchIdCell = ws.Cell(rowIndex, 2);
            churchIdCell.Value = churchId;
            churchIdCell.DataType = XLCellValues.Text;
            CreateBorderStyle(churchIdCell);
        }

        private void CreateNameCell(string name, IXLWorksheet ws, int rowIndex){
            var nameCell = ws.Cell(rowIndex, 3);
            nameCell.Value = name;
            CreateBorderStyle(nameCell);
        }

        private void CreateGroupNameCell(string groupName, IXLWorksheet ws, int rowIndex){
            var groupNameCell = ws.Cell(rowIndex, 4);
            groupNameCell.Value = groupName;
            CreateBorderStyle(groupNameCell);
        }

        private void CreateLogDateCell(AttendanceDayInfo dayInfo, IXLWorksheet ws, int rowIndex)
        {
            var logDateCell = ws.Cell(rowIndex, 5);
            ChangeValueToShorterValue(dayInfo, logDateCell);
            CreateBorderStyle(logDateCell);
        }

        private void ChangeValueToShorterValue(AttendanceDayInfo dayInfo, IXLCell cell){
            if (dayInfo.DayAttendanceStatus != DayAttendanceStatus.None){
                switch (dayInfo.DayAttendanceStatus){
                    case DayAttendanceStatus.Present:
                        DisplayDate(cell, dayInfo);
                        FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                        break;
                    case DayAttendanceStatus.Late:
                        DisplayDate(cell, dayInfo);
                        FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                        break;
                    case DayAttendanceStatus.Absent:
                        cell.Value = " X ";
                        FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                        break;
                    case DayAttendanceStatus.OtherLocal:
                        DisplayDate(cell, dayInfo);
                        FillBackGroudColor(cell, dayInfo.DayAttendanceStatus);
                        break;
                    case DayAttendanceStatus.NA:
                        cell.Value = "N/A";
                        break;
                }
            }
        }

        private void DisplayDate(IXLCell cell, AttendanceDayInfo dayInfo)
        {
            if (IsDateHasHour(dayInfo.DateAndTime)){
                cell.Value = dayInfo.DateAndTimeStr;
                cell.DataType = XLCellValues.DateTime;
                cell.Style.DateFormat.Format = @"[$-409]m/d/yy (h:mm AM/PM);@";
            }
            else{
                cell.Value = dayInfo.DateAndTimeStr;
                cell.DataType = XLCellValues.DateTime;
            }
        }

        private bool IsDateHasHour(DateTime date)
        {
            return date.Hour > 0;
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

        private void CreateBorderStyle(IXLCell cell)
        {
            cell.Style.Border.TopBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.TopBorderColor = XLColor.LightBlue;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.LeftBorderColor = XLColor.LightBlue;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.RightBorderColor = XLColor.LightBlue;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            cell.Style.Border.BottomBorderColor = XLColor.LightBlue;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

    }
}
