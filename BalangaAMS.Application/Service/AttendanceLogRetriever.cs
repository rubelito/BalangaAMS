using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class AttendanceLogRetriever : IAttendanceLogRetriever
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IBrethrenRepository _brethrenRepository;

        public AttendanceLogRetriever(IAttendanceLogRepository attendanceLogRepository,
            IBrethrenRepository brethrenRepository){
            _attendanceLogRepository = attendanceLogRepository;
            _brethrenRepository = brethrenRepository;
        }

        public AttendanceLog GetAttendanceLogById(long id){
            return _attendanceLogRepository.Find(a => a.Id == id).FirstOrDefault();
        }

        public List<AttendanceLog> GetAttendanceLogInSession(GatheringSession gatheringSession){
            return gatheringSession.AttendanceLogs.ToList();
        }

        public AttendanceLog GetBrethrenAttendanceLogInSession(long brethrenId, GatheringSession gatheringSession){
            var attendanceLog = gatheringSession.AttendanceLogs.FirstOrDefault(a => a.BrethrenId == brethrenId);
            return attendanceLog;
        }

        public List<AttendanceLog> GetBrethrenAttendanceLogForMonthOf(long brethrenId, MonthofYear monthofYear, int year){
            var attendaceLogforMonth = _attendanceLogRepository.Find(a => a.BrethrenId == brethrenId
                                                                          &&
                                                                          (MonthofYear) a.DateTime.Month == monthofYear &&
                                                                          a.DateTime.Year == year).ToList();
            return attendaceLogforMonth;
        }

        public BrethrenBasic GetBrethrenInAttendanceLog(long attendanceLogId){
            var attendancelog = _attendanceLogRepository.Find(a => a.Id == attendanceLogId).FirstOrDefault();
            var brethren = _brethrenRepository.Find(b => b.Id == attendancelog.BrethrenId).FirstOrDefault();
            return brethren;
        }

        public void RemoveAttendanceLog(long brethrenId, long sessionId){
            var logs =
                _attendanceLogRepository.Find(a => a.GatheringSession.Id == sessionId && a.BrethrenId == brethrenId)
                    .ToList();
            foreach (var log in logs){
                _attendanceLogRepository.Remove(log);
                _attendanceLogRepository.Commit();
            }
        }
    }
}