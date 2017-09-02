using System.Collections.Generic;
using System.Linq;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain.Enum;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.CustomUI;

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

            CreateSheetThisLocal(prayerMeetingSheet, Gatherings.Prayer_Meeting);
            CreateSheetThisLocal(worshipServiceSheet, Gatherings.Worship_Service);
            CreateSheetThisLocal(thanksGivingSheet, Gatherings.Thanks_Giving);

            IXLWorksheet prayerMeetingSheetOtherLocal = wb.Worksheets.Add("Prayer Meeting - OtherLocal");
            IXLWorksheet worshipServiceSheetOtherLocal = wb.Worksheets.Add("Worship Service - OtherLocal");
            IXLWorksheet thanksGivingSheetOtherLocal = wb.Worksheets.Add("Thanks Giving - OtherLocal");

            CreateSheetForOtherLocal(prayerMeetingSheetOtherLocal, Gatherings.Prayer_Meeting);
            CreateSheetForOtherLocal(worshipServiceSheetOtherLocal, Gatherings.Worship_Service);
            CreateSheetForOtherLocal(thanksGivingSheetOtherLocal, Gatherings.Thanks_Giving);


            wb.SaveAs(gatheringsInMonth.DestinationPath);
        }

        private void CreateSheetThisLocal(IXLWorksheet sheet, Gatherings gathering){
            CreateSheetForGatherings(sheet, gathering, false);
        }

        private void CreateSheetForOtherLocal(IXLWorksheet sheet, Gatherings gathering)
        {
            CreateSheetForGatherings(sheet, gathering, true);
        }

        private void CreateSheetForGatherings(IXLWorksheet sheet, Gatherings gathering, bool isOtherLocal){
            List<GatheringAttendanceInfo> prayerMeetings =
                _report.Gatherings.Where(g => g.Gathering.Gatherings == gathering).ToList();

            const int startColumn = 1;
            const int endComlumn = 2;
            const int initialRow = 2;

            int idNumIndex = startColumn;
            int nameIndex = endComlumn;

            if (isOtherLocal){

                foreach (var g in prayerMeetings)
                {
                    var dayColumn = sheet.Range(1, idNumIndex, 1, idNumIndex).Merge();
                    dayColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dayColumn.Value = g.Gathering.Date.ToString("MM/dd/yyyy");
                    dayColumn.DataType = XLCellValues.DateTime;

                    var idNumColumn = sheet.Cell(initialRow, idNumIndex);
                    idNumColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    idNumColumn.Value = "ID";

                    int currentRow = initialRow + 1;
                    var attendees = g.OtherLocalChurchIds.OrderBy(a => a).ToList();

                    foreach (var a in attendees)
                    {
                        var idNumCell = sheet.Cell(currentRow, idNumIndex);
                        idNumCell.Value = a;
                        idNumCell.DataType = XLCellValues.Text;
                        idNumCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        currentRow++;
                    }
                    idNumIndex = idNumIndex + 2;
                }
            }
            else{ //Brethren in this local
                foreach (var g in prayerMeetings)
                {
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

                    foreach (var a in attendees)
                    {
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
}
