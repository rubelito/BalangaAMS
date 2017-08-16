using System.Collections.Generic;
using System.Data;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IWeeklyReport
    {
        DataTable GetBrethrenReport(long brethrenId, List<GatheringSession> sessionList);
    }
}
