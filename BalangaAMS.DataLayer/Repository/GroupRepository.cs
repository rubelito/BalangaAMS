using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.DataLayer.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AMSDbContext _context;

        public GroupRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(databaseType);
        }

        public void Add(Group group){
            _context.Groups.Add(group);
        }

        public IQueryable<Group> Find(Expression<Func<Group, bool>> predicate){
            return _context.Groups.Where(predicate);
        }

        public IQueryable<Group> FindAll(){
            return _context.Groups;
        }

        public void AddBrethrenToGroup(Group group, long brethrenId){
            var brethren = _context.BrethrenBasics.Find(brethrenId);
            var oldentity = _context.Groups.Find(group.Id);
            group.Brethren.Add(brethren);
            _context.Entry(oldentity).CurrentValues.SetValues(group);
        }

        public void RemoveBrethrenToGroup(Group group, long brethrenId){
            var brethren = _context.BrethrenBasics.Find(brethrenId);
            var oldentity = _context.Groups.Find(group.Id);
            group.Brethren.Remove(brethren);
            _context.Entry(oldentity).CurrentValues.SetValues(group);
        }

        public void Remove(Group group){
            var groupToRemove = _context.Groups.Find(group.Id);
            _context.Groups.Remove(groupToRemove);
        }

        public void Update(Group group){
            var oldentity = _context.Groups.Find(group.Id);
            _context.Entry(oldentity).CurrentValues.SetValues(group);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
