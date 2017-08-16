using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IChurchGatheringRepository
    {
        void Add(GatheringSession churchGathering);
        IQueryable<GatheringSession> Find(Expression<Func<GatheringSession, bool>> predicate);
        IQueryable<GatheringSession> FindAll();
        void Remove(GatheringSession churchGathering);
        void Update(GatheringSession churchGathering);
        void Commit();
    }
}
