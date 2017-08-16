using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IChurchGatheringRetriever
    {
        List<GatheringSession> RetrieveLatestDifferentGatherings();
        GatheringSession GetGatheringById(long id);
        List<GatheringSession> GetGatheringsThatBrethrenAttendedForTheMonthOf(long brethrenId, MonthofYear monthofYear, int year);
        List<GatheringSession> GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(long brethrenId, MonthofYear monthofYear, int year);
        List<GatheringSession> GetAllGatheringsForMonthOf(MonthofYear monthofYear, int year);
        List<GatheringSession> GetAllStartedRegularGatheringsForMonthOf(MonthofYear monthofYear, int year);
        List<GatheringSession> GetNotStartedGatheringsForMonthOf(MonthofYear monthofYear, int year);
        List<GatheringSession> GetStartedGatheringsForMonthOf(MonthofYear monthofYear, int year);
        List<GatheringSession> GetGatheringsThatBrethrenDidntAttendForLast12Session(long brethrenId);
        List<GatheringSession> GetGatheringsThatBrethrenAttendedForLast12Session(long brethrenId);
        List<GatheringSession> GetLast12StartedGatherings();
        List<GatheringSession> FindGatheringSessions(Expression<Func<GatheringSession, bool>> predicate); 
        int CountAllGatheringsInAMonth(MonthofYear monthofYear, int year);
        int CountAllStartedRegularGatheringsInAMonth(MonthofYear monthofYear, int year);
        int CountBrethrenAttendancedInAMonth(long brethrenId, MonthofYear monthofYear, int year);
        int CountBrethrenAbsentInAMonth(long brethrenId, MonthofYear monthofYear, int year);
        bool HasAttendees(long sessionId);
        bool IsAttendedThisGathering(long gatheringId, long brethrenId);
    }
}