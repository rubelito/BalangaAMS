using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using ClosedXML.Excel;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class BrethrenExcelExporter : IExportBrethren
    {
        public void ExportBrethren(List<BrethrenBasic> brethrenList, string destinationPath)
        {
            if (IsBrethrenEmpty(brethrenList))
                throw new ArgumentNullException("Brethren List should not be empty");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid File Path");

            Export(brethrenList, destinationPath);
        }

            private bool IsBrethrenEmpty(List<BrethrenBasic> brethrenList)
            {
                bool isEmpty = brethrenList == null || brethrenList.Count == 0;
                return isEmpty;
            }

        private void Export(List<BrethrenBasic> brethrenList, string destinationPath)
            {
                var maleBrethren = brethrenList.Where(b => b.BrethrenFull.Gender == Gender.Male).ToList();
                var femaleBrethren = brethrenList.Where(b => b.BrethrenFull.Gender == Gender.Female).ToList();

                var wb = new XLWorkbook();
                var maleSheet = wb.Worksheets.Add("MALE");
                var femaleSheet = wb.Worksheets.Add("FEMALE");

                CreateSheet(maleSheet, maleBrethren);
                CreateSheet(femaleSheet, femaleBrethren);

                wb.SaveAs(destinationPath);
            }

                private void CreateSheet(IXLWorksheet worksheet, List<BrethrenBasic> brethrenList)
                {
                    int rowIndex = 2;
                    CreateColumn(worksheet);
                    foreach (var brethren in brethrenList)
                    {
                        CreateRows(worksheet, brethren, rowIndex);
                        rowIndex++;
                    }
                }

                    private void CreateColumn(IXLWorksheet w)
                    {
                        w.Cell(1, "A").Value = "Group";
                        w.Cell(1, "B").Value = "Name";
                        w.Cell(1, "C").Value = "NickName";
                        w.Cell(1, "D").Value = "ChurchId";
                        w.Cell(1, "E").Value = "DateofBaptism";
                        w.Cell(1, "F").Value = "PlaceofBaptism";
                        w.Cell(1, "G").Value = "Baptizer";
                        w.Cell(1, "H").Value = "Contactno";
                        w.Cell(1, "I").Value = "StreetNumber";
                        w.Cell(1, "J").Value = "Street";
                        w.Cell(1, "K").Value = "Barangay";
                        w.Cell(1, "L").Value = "Town";
                        w.Cell(1, "M").Value = "Province";
                        w.Cell(1, "N").Value = "Language";
                        w.Cell(1, "O").Value = "DateofBirth";
                        w.Cell(1, "P").Value = "Gender";
                        w.Cell(1, "Q").Value = "CivilStatus";
                        w.Cell(1, "R").Value = "Job";
                        w.Cell(1, "S").Value = "Skills";
                        w.Cell(1, "T").Value = "EducationalAttainment";
                    }

                    private void CreateRows(IXLWorksheet w, BrethrenBasic b, int rowIndex)
                    {
                        w.Cell(rowIndex, "A").Value = b.Group != null ? b.Group.GroupName : "";
                        w.Cell(rowIndex, "B").Value = b.Name;
                        w.Cell(rowIndex, "C").Value = b.BrethrenFull.NickName;
                        w.Cell(rowIndex, "D").Value = b.ChurchId;
                        w.Cell(rowIndex, "D").DataType = XLCellValues.Text;
                        w.Cell(rowIndex, "E").Value = b.BrethrenFull.DateofBaptism.HasValue
                            ? b.BrethrenFull.DateofBaptism.Value.ToShortDateString()
                            : "";
                        w.Cell(rowIndex, "F").Value = b.BrethrenFull.PlaceofBaptism;
                        w.Cell(rowIndex, "G").Value = b.BrethrenFull.Baptizer;
                        w.Cell(rowIndex, "H").Value = b.BrethrenFull.Contactno;
                        w.Cell(rowIndex, "H").DataType = XLCellValues.Text;
                        w.Cell(rowIndex, "I").Value = b.BrethrenFull.StreetNumber;
                        w.Cell(rowIndex, "I").DataType = XLCellValues.Text;
                        w.Cell(rowIndex, "J").Value = b.BrethrenFull.Street;
                        w.Cell(rowIndex, "K").Value = b.BrethrenFull.Barangay;
                        w.Cell(rowIndex, "L").Value = b.BrethrenFull.Town;
                        w.Cell(rowIndex, "M").Value = b.BrethrenFull.Province;
                        w.Cell(rowIndex, "N").Value = b.BrethrenFull.Language;
                        w.Cell(rowIndex, "O").Value = b.BrethrenFull.DateofBirth.HasValue
                            ? b.BrethrenFull.DateofBirth.Value.ToShortDateString()
                            : "";
                        w.Cell(rowIndex, "P").Value = b.BrethrenFull.Gender.ToString();
                        w.Cell(rowIndex, "Q").Value = b.BrethrenFull.CivilStatus.ToString();
                        w.Cell(rowIndex, "R").Value = b.BrethrenFull.Job;
                        w.Cell(rowIndex, "S").Value = b.BrethrenFull.Skills;
                        w.Cell(rowIndex, "T").Value = b.BrethrenFull.EducationalAttainment;
                    }       
    }
}