using System.Collections.Generic;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IAttendanceLogRetriever
    {
        AttendanceLog GetAttendanceLogById(long id);
        List<AttendanceLog> GetAttendanceLogInSession(GatheringSession gatheringSession);
        AttendanceLog GetBrethrenAttendanceLogInSession(long brethrenId, GatheringSession gatheringSession);
        List<AttendanceLog> GetBrethrenAttendanceLogForMonthOf(long brethrenId, MonthofYear monthofYear, int year);
        BrethrenBasic GetBrethrenInAttendanceLog(long attendanceLogId);
        void RemoveAttendanceLog(long brethrenId, long sessionId);
    }
}