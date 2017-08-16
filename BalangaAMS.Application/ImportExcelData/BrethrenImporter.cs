using BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData;
using BalangaAMS.Core.HelperDomain;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BalangaAMS.ApplicationLayer.ImportExcelData
{
    public class BrethrenImporter : IImportbrethren
    {
        private readonly string _filepath;

        public BrethrenImporter(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("Members Masterlist file not found!");
            }
            _filepath = filepath;
        }

        public List<DatatoImport> Loadbretrhen()
        {
            var male = ReadMaleSheet();
            var female = ReadFemaleSheet();
            var brethren = male.Concat(female);
            return brethren.ToList();
        }
       
        private List<DatatoImport> ReadMaleSheet()
        {
            var excel = new ExcelQueryFactory(_filepath);
            List<DatatoImport> male;
            var columns = excel.WorksheetRangeNoHeader("A1", "T10", "MALE").FirstOrDefault();            
            if (columns.Any(b => b.Value.ToString() == "Group") && columns.Any(b => b.Value.ToString() == "ChurchId"))
            {
                male = excel.Worksheet<DatatoImport>("MALE").ToList();
            }
            else
            {
                throw new Exception("Cannot read Excel file: MALE Sheet wrong format");
            }

            return male;
        }

        private List<DatatoImport> ReadFemaleSheet()
        {
            var excel = new ExcelQueryFactory(_filepath);
            List<DatatoImport> female;
            var columns = excel.WorksheetRangeNoHeader("A1", "T10", "FEMALE").FirstOrDefault();

            if (columns.Any(b => b.Value.ToString() == "Group") && columns.Any(b => b.Value.ToString() == "ChurchId"))
            {
                female = excel.Worksheet<DatatoImport>("FEMALE").ToList();
            }
            else
            {
                throw new Exception("Cannot read Excel file: FEMALE sheet wrong format");
            }
            
            return female;
        }
    }
}
