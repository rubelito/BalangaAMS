using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IGroupRepository
    {
        void Add(Group group);
        IQueryable<Group> Find(Expression<Func<Group, bool>> predicate);
        IQueryable<Group> FindAll();
        void AddBrethrenToGroup(Group group, long brethrenId);
        void RemoveBrethrenToGroup(Group group, long brethrenId);
        void Remove(Group group);
        void Update(Group group);
        void Commit();
    }
}
