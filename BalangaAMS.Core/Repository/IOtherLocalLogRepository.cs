using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IOtherLocalLogRepository
    {
        void Add(OtherLocalLog attendanceLog);
        IQueryable<OtherLocalLog> Find(Expression<Func<OtherLocalLog, bool>> predicate);
        IQueryable<OtherLocalLog> FindAll();
        void Remove(OtherLocalLog attendanceLog);
        void Update(OtherLocalLog attendanceLog);
        void Commit();
    }
}
