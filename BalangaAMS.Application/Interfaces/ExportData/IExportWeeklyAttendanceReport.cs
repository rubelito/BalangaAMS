
using BalangaAMS.ApplicationLayer.ExportData;

namespace BalangaAMS.ApplicationLayer.Interfaces.ExportData
{
    public interface IExportWeeklyAttendanceReport
    {
        void ExportWeeklyAttendanceReport(WeeklyAttendanceGroupInfo reportInfo);
    }
}
