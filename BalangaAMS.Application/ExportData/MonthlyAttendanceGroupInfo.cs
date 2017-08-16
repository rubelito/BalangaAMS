using System;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.Core.HelperDomain;
using System.Data;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class MonthlyAttendanceGroupInfo
    {
        private readonly DataTable _reportTable;
        private readonly string _destinationPath;

        public MonthlyAttendanceGroupInfo(DataTable reportTable, string destinationPath)
        {
            if (IsTableIsNullOrEmpty(reportTable))
                throw new ArgumentNullException("Report Table should not be null or empty");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid File Path");
            
            _reportTable = reportTable;
            _destinationPath = destinationPath;
        }

        public MonthlyAttendanceGroupInfo(DataTable reportTable, string destinationPath, MonthofYear monthofYear, 
            int year, string groupName, string divisionName, string districtName)
        {
            if (IsTableIsNullOrEmpty(reportTable))
                throw new ArgumentNullException("Report Table should not be null or empty");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid file path or file path");

            _reportTable = reportTable;
            _destinationPath = destinationPath;
            MonthofYear = monthofYear;
            Year = year;
            GroupName = groupName;
            DivisionName = divisionName;
            DistrictName = districtName;
        }

        private bool IsTableIsNullOrEmpty(DataTable reportTableList)
        {
            bool isEmpty = reportTableList == null || reportTableList.Rows.Count == 0;
            return isEmpty;
        }

        public DataTable ReportTable { get {return _reportTable; } }
        public string DestinationPath { get { return _destinationPath; } }
        public MonthofYear MonthofYear { get; set; }
        public int Year { get; set; }
        public string GroupName { get; set; }
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
    }
}
