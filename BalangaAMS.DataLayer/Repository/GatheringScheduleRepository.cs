using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.DataLayer.Repository
{
    public class GatheringScheduleRepository : IGatheringScheduleRepository
    {
        private readonly AMSDbContext _context;

        public GatheringScheduleRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(databaseType);
        }

        public void Add(GatheringSchedule schedule){
            _context.GatheringSchedules.Add(schedule);
        }

        public IQueryable<GatheringSchedule> Find(Expression<Func<GatheringSchedule, bool>> predicate){
            return _context.GatheringSchedules.Where(predicate);
        }

        public IQueryable<GatheringSchedule> FindAll(){
            return _context.GatheringSchedules;
        }

        public void Remove(GatheringSchedule schedule){
            var scheduleToRemove = _context.GatheringSchedules.Find(schedule.Id);
            _context.GatheringSchedules.Remove(scheduleToRemove);
        }

        public void Update(GatheringSchedule schedule){
            var oldentity = _context.GatheringSchedules.Find(schedule.Id);
            _context.Entry(oldentity).CurrentValues.SetValues(schedule);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
