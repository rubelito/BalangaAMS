using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.DataLayer.Repository
{
    public class ChurchGatheringRepository : IChurchGatheringRepository
    {
        private readonly AMSDbContext _context;

        public ChurchGatheringRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context =new AMSDbContext(databaseType);
        }

        public void Add(GatheringSession entity){
            _context.GatheringSessions.Add(entity);
        }

        public IQueryable<GatheringSession> Find(Expression<Func<GatheringSession, bool>> predicate){
            return _context.GatheringSessions.Where(predicate);
        }

        public IQueryable<GatheringSession> FindAll(){
            return _context.GatheringSessions;
        }

        public void Remove(GatheringSession entity){
            var entityToRemove = _context.GatheringSessions.Find(entity.Id);
            _context.GatheringSessions.Remove(entityToRemove);
        }

        public void Update(GatheringSession entity){
            var oldentity = _context.GatheringSessions.Find(entity.Id);
            _context.Entry(oldentity).CurrentValues.SetValues(entity);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
