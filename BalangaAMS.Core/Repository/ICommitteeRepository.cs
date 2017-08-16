using System;
using System.Linq;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.Core.Repository
{
    public interface ICommitteeRepository
    {
        void Add(Committee committee);
        IQueryable<Committee> Find(Expression<Func<Committee, bool>> predicate);
        IQueryable<Committee> FindAll();
        void Remove(Committee committee);
        void Update(Committee committee);
        void Commit();
    }
}
