using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IAttendanceLogRepository
    {
        void Add(AttendanceLog attendanceLog);
        IQueryable<AttendanceLog> Find(Expression<Func<AttendanceLog, bool>> predicate);
        IQueryable<AttendanceLog> FindAll();
        void Remove(AttendanceLog attendanceLog);
        void Update(AttendanceLog attendanceLog);
        void Commit();
    }
}
