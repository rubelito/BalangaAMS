using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class AttendeesRetriever : IAttendeesRetriever
    {
        private readonly IAttendanceRetriever _attendanceRetriever;

        public AttendeesRetriever(IAttendanceRetriever attendanceRetriever){
            _attendanceRetriever = attendanceRetriever;
        }

        public List<GatheringAttendanceInfo> GetAttendees(List<GatheringSession> gatherings){
            var gAttendanceInfos = new List<GatheringAttendanceInfo>();
            foreach (var ga in gatherings){
                var gaInfos = new GatheringAttendanceInfo();
                gaInfos.Gathering = ga;
                gaInfos.Attendees = _attendanceRetriever.GetBrethrenWhoAttendedThisGathering(ga);
                gaInfos.OtherLocalChurchIds = _attendanceRetriever.GetOtherLocalWhoAttendedThisGathering(ga);
                gAttendanceInfos.Add(gaInfos);
            }

            return gAttendanceInfos;
        }
    }
}
