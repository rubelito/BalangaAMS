using System.Collections.Generic;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.Core.Interfaces
{
    public interface ILateIdentifier
    {
        int CountTheLateOfBrethrenForMonthOf(long brethrenId, MonthofYear monthofYear, int year);
        List<BrethrenBasic> GetLateBrethrensInSession(long gatheringSessionId);
        bool IsBrethrenIsLate(long brethrenId, long gatheringSessionId);
    }
}