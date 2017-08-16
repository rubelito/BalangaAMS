using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.DataLayer.Repository
{
    public class AttendanceLogRepository : IAttendanceLogRepository
    {
        private readonly AMSDbContext _context;

        public AttendanceLogRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(databaseType);
        }

        public void Add(AttendanceLog attendanceLog){
            _context.AttendanceLogs.Add(attendanceLog);
        }

        public IQueryable<AttendanceLog> Find(Expression<Func<AttendanceLog, bool>> predicate){
            return _context.AttendanceLogs.Where(predicate);
        }

        public IQueryable<AttendanceLog> FindAll(){
            return _context.AttendanceLogs;
        }

        public void Remove(AttendanceLog attendanceLog){
            var entityToRemove = _context.AttendanceLogs.Find(attendanceLog.Id);
            _context.AttendanceLogs.Remove(entityToRemove);
        }

        public void Update(AttendanceLog attendanceLog){
            var oldentity = _context.AttendanceLogs.Find(attendanceLog.Id);
            _context.Entry(oldentity).CurrentValues.SetValues(attendanceLog);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
