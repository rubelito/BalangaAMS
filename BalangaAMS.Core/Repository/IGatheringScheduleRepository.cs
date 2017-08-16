using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IGatheringScheduleRepository
    {
        void Add(GatheringSchedule schedule);
        IQueryable<GatheringSchedule> Find(Expression<Func<GatheringSchedule, bool>> predicate);
        IQueryable<GatheringSchedule> FindAll();
        void Remove(GatheringSchedule schedule);
        void Update(GatheringSchedule schedule);
        void Commit();
    }
}
