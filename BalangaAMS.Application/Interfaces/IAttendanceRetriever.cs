using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Interfaces
{
    public interface IAttendanceRetriever
    {
        List<BrethrenBasic> GetBrethrenWhoAttendedThisGathering(GatheringSession session);
        List<BrethrenBasic> GetBrethrenWhoIsAbsentToThisGathering(GatheringSession session);
        List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(GatheringSession session);
        List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLive(GatheringSession session);
        List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringNotLive(GatheringSession session);
        List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLate(GatheringSession session);
        List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoIsAbsentInThisGathering(GatheringSession session);
        bool IsAlreadyLogin(long brethrenId, GatheringSession session);
    }
}
