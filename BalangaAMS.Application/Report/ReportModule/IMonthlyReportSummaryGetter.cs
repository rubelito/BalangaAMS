using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.ApplicationLayer.Report.ReportModule
{
    public interface IMonthlyReportSummaryGetter
    {
        MontlyReportSummary GetSummaryReport(Group group, MonthofYear monthofYear, int year);
        IndividualReportSummary GetIndividualReportSummary(MonthofYear monthofYear, int year);
    }
}