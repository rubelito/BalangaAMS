using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.ApplicationLayer.ExportData
{
    public class FinalAttendanceReport
    {
        public List<GatheringAttendanceInfo> Gatherings { get; set; } 
        public MonthofYear Month { get; set; }
        public int Year { get; set; }
        public string DestinationPath { get; set; }
    }
}
