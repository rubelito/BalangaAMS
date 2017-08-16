using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IScheduleManager
    {
        void AddSchedule(GatheringSchedule gatheringSchedule);
        void UpdateSchedule(GatheringSchedule gatheringSchedule);
        List<GatheringSchedule> GetListofSchedule();
        void RemoveSchedule(GatheringSchedule gatheringSchedule);
    }
}