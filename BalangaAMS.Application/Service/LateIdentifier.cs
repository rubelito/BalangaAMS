using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class LateIdentifier : ILateIdentifier
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IAttendanceLogRetriever _logRetriever;

        public LateIdentifier(IChurchGatheringRetriever sessionRetriever, IAttendanceLogRetriever logRetriever)
        {
            _sessionRetriever = sessionRetriever;
            _logRetriever = logRetriever;
        }

        public int CountTheLateOfBrethrenForMonthOf(long brethrenId, MonthofYear monthofYear, int year)
        {
            var attendedGatheringSession = _sessionRetriever
                .GetGatheringsThatBrethrenAttendedForTheMonthOf(brethrenId, monthofYear, year);
            var lateAttendancelog = new List<AttendanceLog>();
            foreach (var gatheringSession in attendedGatheringSession)
            {
                var attendancelog = _logRetriever
                    .GetBrethrenAttendanceLogInSession(brethrenId, gatheringSession);
                if (attendancelog.IsLate)
                {
                    lateAttendancelog.Add(attendancelog);
                }
            }
            return lateAttendancelog.Count;
        }

        public List<BrethrenBasic> GetLateBrethrensInSession(long gatheringSessionId)
        {
            var gatheringSession = _sessionRetriever.GetGatheringById(gatheringSessionId);
            var lateAttendanceLog = gatheringSession.AttendanceLogs.Where(a => a.IsLate).ToList();
            var brethrenList = new List<BrethrenBasic>();

            foreach (var attendanceLog in lateAttendanceLog)
            {
                brethrenList.Add(_logRetriever.GetBrethrenInAttendanceLog(attendanceLog.Id));
            }
            return brethrenList;
        }

        public bool IsBrethrenIsLate(long brethrenId, long gatheringSessionId)
        {
            var gatheringSession = _sessionRetriever.GetGatheringById(gatheringSessionId);
            var attendancelog = _logRetriever.GetBrethrenAttendanceLogInSession(brethrenId, gatheringSession);
            return attendancelog.IsLate;
        }
    }
}