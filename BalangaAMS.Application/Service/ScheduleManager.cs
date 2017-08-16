using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class ScheduleManager : IScheduleManager
    {
        private readonly IGatheringScheduleRepository _repo;

        public ScheduleManager(IGatheringScheduleRepository repo){
            _repo = repo;
        }

        public void AddSchedule(GatheringSchedule gatheringSchedule){
            _repo.Add(gatheringSchedule);
            _repo.Commit();
        }

        public void UpdateSchedule(GatheringSchedule gatheringSchedule){
            _repo.Update(gatheringSchedule);
            _repo.Commit();
        }

        public List<GatheringSchedule> GetListofSchedule(){
            return _repo.FindAll().ToList();
        }

        public void RemoveSchedule(GatheringSchedule gatheringSchedule){
            _repo.Remove(gatheringSchedule);
            _repo.Commit();
        }
    }
}
