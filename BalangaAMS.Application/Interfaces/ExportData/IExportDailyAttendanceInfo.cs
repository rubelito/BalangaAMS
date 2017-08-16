using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Interfaces.ExportData
{
    public interface IExportDailyAttendanceInfo
    {
        void ExportDailyAttendanceInfo(List<AttendanceInfoDTO> attendanceDTOList, GatheringSession session,string title,string destinationPath);
    }
}
