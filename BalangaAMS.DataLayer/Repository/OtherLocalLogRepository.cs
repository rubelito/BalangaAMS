using System;
using System.Linq;
using BalangaAMS.Core.Repository;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.DataLayer.EntityFramework;

namespace BalangaAMS.DataLayer.Repository
{
    public class OtherLocalLogRepository : IOtherLocalLogRepository
    {
        private readonly AMSDbContext _context;

        public OtherLocalLogRepository(IDatabaseType dbType){
            if (dbType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(dbType);
        }

        public void Add(Core.Domain.OtherLocalLog attendanceLog){
            _context.OtherLocalLogs.Add(attendanceLog);
        }

        public IQueryable<Core.Domain.OtherLocalLog> Find(System.Linq.Expressions.Expression<Func<Core.Domain.OtherLocalLog, bool>> predicate){
            return _context.OtherLocalLogs.Where(predicate);
        }

        public IQueryable<Core.Domain.OtherLocalLog> FindAll(){
            return _context.OtherLocalLogs;
        }

        public void Remove(Core.Domain.OtherLocalLog attendanceLog){
            var logToRemove = _context.OtherLocalLogs.Find(attendanceLog.Id);
            _context.OtherLocalLogs.Remove(logToRemove);
        }

        public void Update(Core.Domain.OtherLocalLog attendanceLog)
        {
            throw new NotImplementedException();
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
