using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service.GatheringSessionManager
{
    public class ChurchGatheringRetriever : IReturnStatus, IChurchGatheringRetriever
    {
        private readonly IChurchGatheringRepository _gatheringRepository;
        private readonly IBrethrenRepository _brethrenRepository;
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private string _statusmessage = "No status";

        public ChurchGatheringRetriever(IChurchGatheringRepository churchGatheringRepository,
            IBrethrenRepository brethrenRepository, IAttendanceLogRepository attendanceLogRepository){
            _gatheringRepository = churchGatheringRepository;
            _brethrenRepository = brethrenRepository;
            _attendanceLogRepository = attendanceLogRepository;
        }

        public List<GatheringSession> RetrieveLatestDifferentGatherings(){
            var gatheringSessions = new List<GatheringSession>();
            try{
                foreach (var gathering in Enum.GetValues(typeof (Gatherings)).Cast<Gatherings>()){
                    var gs =
                        _gatheringRepository.FindAll()
                            .Where(g => g.Gatherings == gathering && g.IsStarted)
                            .OrderByDescending(g => g.Date).FirstOrDefault();

                    if (gs != null){
                        gatheringSessions.Add(gs);
                    }
                }
                _statusmessage = "Success getting GatheringSessions";
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
                return null;
            }

            return gatheringSessions;
        }

        public GatheringSession GetGatheringById(long id){
            GatheringSession gatheringsession = null;
            try{
                gatheringsession = _gatheringRepository.Find(g => g.Id == id).FirstOrDefault();
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
            }

            return gatheringsession;
        }

        public List<GatheringSession> GetGatheringsThatBrethrenAttendedForTheMonthOf(long brethrenId,
            MonthofYear monthofYear, int year){
            var gatheringsessionattended = _attendanceLogRepository.Find(a => a.BrethrenId == brethrenId
                                                                              &&
                                                                              (MonthofYear)
                                                                                  a.GatheringSession.Date.Month ==
                                                                              monthofYear &&
                                                                              a.GatheringSession.Date.Year == year &&
                                                                              a.GatheringSession.IsStarted &&
                                                                              a.GatheringSession.Gatherings !=
                                                                              Gatherings.PNK)
                .Select(at => at.GatheringSession)
                .Distinct()
                .ToList();

            return gatheringsessionattended;
        }

        public List<GatheringSession> GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(long brethrenId,
            MonthofYear monthofYear, int year){
            var brethren = _brethrenRepository.Find(b => b.Id == brethrenId).FirstOrDefault();

            var gatheringAttended = GetGatheringsThatBrethrenAttendedForTheMonthOf(brethrenId, monthofYear, year);

            var gatheringSessionThatNotAttended =
                GetAllStartedRegularGatheringsForMonthOf(monthofYear, year)
                    .Where(g =>
                        g.Date >= brethren.LastStatusUpdate &&
                        gatheringAttended.All(ga => ga.Id != g.Id)).ToList();

            return gatheringSessionThatNotAttended;
        }

        public List<GatheringSession> GetAllGatheringsForMonthOf(MonthofYear monthofYear, int year){
            return _gatheringRepository.Find(g => (MonthofYear) g.Date.Month == monthofYear &&
                                                  g.Date.Year == year).ToList();
        }

        public List<GatheringSession> GetStartedGatheringsForMonthOf(MonthofYear monthofYear, int year){
            return _gatheringRepository.Find(g => (MonthofYear) g.Date.Month == monthofYear &&
                                                  g.Date.Year == year && g.IsStarted).ToList();
        }

        public List<GatheringSession> GetAllStartedRegularGatheringsForMonthOf(MonthofYear monthofYear, int year){
            return _gatheringRepository.Find(g => (MonthofYear)g.Date.Month == monthofYear &&
                                                  g.Date.Year == year && g.IsStarted && g.Gatherings != Gatherings.PNK).ToList();
        }

        public List<GatheringSession> GetNotStartedGatheringsForMonthOf(MonthofYear monthofYear, int year){
            return _gatheringRepository.Find(g => (MonthofYear) g.Date.Month == monthofYear &&
                                                  g.Date.Year == year && g.IsStarted == false).ToList();
        }

        public List<GatheringSession> GetGatheringsThatBrethrenDidntAttendForLast12Session(long brethrenId){
            var last12GatheringSession = GetLast12StartedGatherings();
            var gatheringSessionAttended = GetGatheringsThatBrethrenAttendedForLast12Session(brethrenId);
            var gatheringSessionNotAttended = last12GatheringSession.Except(gatheringSessionAttended).ToList();

            return gatheringSessionNotAttended;
        }

        public List<GatheringSession> GetGatheringsThatBrethrenAttendedForLast12Session(long brethrenId){
            var gatheringSessionAttended = _attendanceLogRepository.Find(a => a.BrethrenId == brethrenId)
                .Select(at => at.GatheringSession)
                .OrderByDescending(g => g.Date)
                .Take(12)
                .ToList();

            return gatheringSessionAttended;
        }

        public List<GatheringSession> GetLast12StartedGatherings(){
            return _gatheringRepository.Find(g => g.IsStarted).OrderByDescending(g => g.Date).Take(12).ToList();
        }

        public List<GatheringSession> FindGatheringSessions(Expression<Func<GatheringSession, bool>> predicate){
            return _gatheringRepository.Find(predicate).ToList();
        }

        public int CountAllGatheringsInAMonth(MonthofYear monthofYear, int year){
            return GetAllGatheringsForMonthOf(monthofYear, year).Count();
        }

        public int CountAllStartedRegularGatheringsInAMonth(MonthofYear monthofYear, int year){
            return GetAllStartedRegularGatheringsForMonthOf(monthofYear, year).Count;
        }

        public int CountBrethrenAbsentInAMonth(long brethrenId, MonthofYear monthofYear, int year){
            var gatheringSessionThatNotAttended = GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(brethrenId,
                monthofYear, year);

            return gatheringSessionThatNotAttended.Count;
        }

        public int CountBrethrenAttendancedInAMonth(long brethrenId, MonthofYear monthofYear, int year){
            var gatheringsessionattended = GetGatheringsThatBrethrenAttendedForTheMonthOf(brethrenId, monthofYear, year);

            return gatheringsessionattended.Count;
        }

        public bool HasAttendees(long sessionId){
            var session = GetGatheringById(sessionId);
            return session.AttendanceLogs.Any();
        }

        public bool HasOtherLocalAttendees(long sessionId){
            var session = GetGatheringById(sessionId);
            return session.OtherLocalLogs.Any();
        }

        public bool IsAttendedThisGathering(long gatheringId, long brethrenId){
            var session = GetGatheringById(gatheringId);
            if (session != null){
                return session.AttendanceLogs.Any(a => a.BrethrenId == brethrenId);
            }
            return false;
        }

        public string Statusmessage(){
            return _statusmessage;
        }
    }
}