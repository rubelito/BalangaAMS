using System.Collections.Generic;
using System.Linq;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain.Enum;
using ClosedXML.Excel;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class FinalMonthlyAttendanceReportExporter
    {
        private FinalAttendanceReport _report;

        public void Export(FinalAttendanceReport gatheringsInMonth){
            _report = gatheringsInMonth;
            var wb = new XLWorkbook();
            IXLWorksheet prayerMeetingSheet = wb.Worksheets.Add("Prayer Meeting");
            IXLWorksheet worshipServiceSheet = wb.Worksheets.Add("Worship Service");
            IXLWorksheet thanksGivingSheet = wb.Worksheets.Add("Thanks Giving");

            CreateSheetForGatherings(prayerMeetingSheet, Gatherings.Prayer_Meeting);
            CreateSheetForGatherings(worshipServiceSheet, Gatherings.Worship_Service);
            CreateSheetForGatherings(thanksGivingSheet, Gatherings.Thanks_Giving);
            wb.SaveAs(gatheringsInMonth.DestinationPath);
        }

        private void CreateSheetForGatherings(IXLWorksheet sheet, Gatherings gathering){
            List<GatheringAttendanceInfo> prayerMeetings =
                _report.Gatherings.Where(g => g.Gathering.Gatherings == gathering).ToList();

            int startColumn = 1;
            int endComlumn = 2;
            int initialRow = 2;

            int idNumIndex = startColumn;
            int nameIndex = endComlumn;

            foreach (var g in prayerMeetings){
                var dayColumn = sheet.Range(1, idNumIndex, 1, nameIndex).Merge();
                dayColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dayColumn.Value = g.Gathering.Date.ToString("MM/dd/yyyy");
                dayColumn.DataType = XLCellValues.DateTime;

                var idNumColumn = sheet.Cell(initialRow, idNumIndex);
                idNumColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                idNumColumn.Value = "ID";
                
                var nameColumn = sheet.Cell(initialRow, nameIndex);
                nameColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                nameColumn.Value = "Name";

                int currentRow = initialRow + 1;
                var attendees = g.Attendees.OrderBy(a => a.Name).ToList();

                foreach (var a in attendees){
                    var idNumCell = sheet.Cell(currentRow, idNumIndex);
                    idNumCell.Value = a.ChurchId;
                    idNumCell.DataType = XLCellValues.Text;
                    idNumCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var nameCell = sheet.Cell(currentRow, nameIndex);
                    nameCell.Value = a.Name;
                    nameCell.DataType = XLCellValues.Text;

                    currentRow++;                    
                }
                idNumIndex = idNumIndex + 2;
                nameIndex = nameIndex + 2;
            }
        }
    }
}
