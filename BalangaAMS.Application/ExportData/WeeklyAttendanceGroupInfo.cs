using System;
using System.Data;
using BalangaAMS.ApplicationLayer.HelperClass;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class WeeklyAttendanceGroupInfo
    {
        private readonly DataTable _reportTable;
        private readonly string _destinationPath;

        public WeeklyAttendanceGroupInfo(DataTable reportTable, string destinationPath)
        {
            if (IsTableIsNullOrEmpty(reportTable))
                throw new ArgumentNullException("Report Table should not be null or empty");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid File Path");
            
            _reportTable = reportTable;
            _destinationPath = destinationPath;
        }

        public WeeklyAttendanceGroupInfo(DataTable reportTable, string destinationPath, string dateCoverage, 
            string groupName, string divisionName, string districtName)
        {
            if (IsTableIsNullOrEmpty(reportTable))
                throw new ArgumentNullException("Report Table should not be null or empty");
            if (FileNameChecker.IsNotValidPathOrFileName(destinationPath))
                throw new Exception("Invalid file path or file path");

            _reportTable = reportTable;
            _destinationPath = destinationPath;
            DateCoverage = dateCoverage;
            GroupName = groupName;
            DivisionName = divisionName;
            DistrictName = districtName;
        }

        private bool IsTableIsNullOrEmpty(DataTable reportTableList)
        {
            bool isEmpty = reportTableList == null || reportTableList.Rows.Count == 0;
            return isEmpty;
        }

        public DataTable ReportTable { get { return _reportTable; }  }
        public string DestinationPath { get { return _destinationPath; } }
        public string GroupName { get; set; }
        public string DateCoverage { get; set; }
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
    }
}
