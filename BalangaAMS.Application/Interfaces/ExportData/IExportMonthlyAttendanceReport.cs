using System.Data;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.ApplicationLayer.Interfaces.ExportData
{
    public interface IExportMonthlyAttendanceReport
    {
        void ExportMonthlyAttendanceReport(MonthlyAttendanceGroupInfo reportInfo);
    }
}
