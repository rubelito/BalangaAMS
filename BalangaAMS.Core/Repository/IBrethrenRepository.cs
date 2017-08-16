using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IBrethrenRepository
    {
        void Add(BrethrenBasic brethren);
        IQueryable<BrethrenBasic> Find(Expression<Func<BrethrenBasic, bool>> predicate);
        IQueryable<BrethrenBasic> FindAll();      
        void Remove(BrethrenBasic brethren);
        void Update(BrethrenBasic brethren);
        void Commit();
    }
}
