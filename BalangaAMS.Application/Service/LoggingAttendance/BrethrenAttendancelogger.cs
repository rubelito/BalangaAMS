using System;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using System.Linq;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service.LoggingAttendance
{
    public class BrethrenAttendancelogger:IAttendanceLogger
    {
        private readonly IChurchGatheringRepository _churchGatheringRepository;
        private string _message;
        private bool _isSuccessfulLogging;

        public BrethrenAttendancelogger(IChurchGatheringRepository churchGatheringRepository)
        {
            _churchGatheringRepository = churchGatheringRepository;
        }

        public void Logbrethren(long gatheringSessionId, AttendanceLog attendanceLog)
        {
            try
            {
                var gatheringSession = _churchGatheringRepository.Find(g => g.Id == gatheringSessionId).FirstOrDefault();
                if (gatheringSession != null)
                {
                    gatheringSession.AttendanceLogs.Add(attendanceLog);
                    _churchGatheringRepository.Commit();
                    _isSuccessfulLogging = true;
                    _message = "Success logging";
                }
                else
                {
                    throw new Exception("gathering Session is null!");
                }
            }
            catch (Exception exception)
            {
                _isSuccessfulLogging = false;
                _message = exception.Message;
            }
        }

        public void LogLateBrethren(long gatheringSessionId, AttendanceLog attendanceLog)
        {
            try
            {
                var gatheringSession = _churchGatheringRepository.Find(g => g.Id == gatheringSessionId).FirstOrDefault();
                attendanceLog.IsLate = true;
                if (gatheringSession != null)
                {
                    gatheringSession.AttendanceLogs.Add(attendanceLog);
                    _churchGatheringRepository.Commit();
                    _isSuccessfulLogging = true;
                    _message = "Success logging"; 
                }
                else
                {
                    throw new Exception("gathering sessin is null!");
                }
            }
            catch (Exception exception)
            {
                _isSuccessfulLogging = false;
                _message = exception.Message;
            }
        }

        public bool IsSuccessfulLogging()
        {
            return _isSuccessfulLogging;
        }

        public string Statusmessage()
        {
            return _message;
        }
    }
}