using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Interfaces
{
    public interface IAttendeesRetriever
    {
        List<GatheringAttendanceInfo> GetAttendees(List<GatheringSession> gatherings);
    }
}