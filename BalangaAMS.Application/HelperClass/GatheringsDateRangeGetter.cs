using System.Collections.Generic;
using System.Data;
using System.Linq;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.HelperClass
{
    public class GatheringsDateArranger
    {
        public static string GetDateCoverage(List<GatheringSession> sessions)
        {
            if (sessions == null)
                throw new NoNullAllowedException("List<Gatherings> should not be null");

            var arangedSessions = sessions.OrderBy(s => s.Date).ToList();

            var dateCoverage = arangedSessions.FirstOrDefault().Date.ToString("M") + " - " +
                   arangedSessions.LastOrDefault().Date.ToString("MMMM dd, yyyy");
            return dateCoverage;
        }

        public static List<GatheringSession> ArrangeSessionsByDate(List<GatheringSession> sessions)
        {
            return sessions.OrderBy(s => s.Date).ToList();
        }
    }
}
