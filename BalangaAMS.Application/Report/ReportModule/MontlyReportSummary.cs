namespace BalangaAMS.ApplicationLayer.Report.ReportModule
{
    public class MontlyReportSummary
    {
        public string GroupName { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public string MonthofYear { get; set; }
        public int GatheringsTotal { get; set; }
    }
}