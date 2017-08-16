using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;
using System.Linq.Expressions;

namespace BalangaAMS.DataLayer.Repository
{
    public class CommitteeRepository : ICommitteeRepository
    {
        private readonly AMSDbContext _context;

        public CommitteeRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(databaseType);
        }

        public void Add(Committee committee){
            _context.Committees.Add(committee);
        }

        public IQueryable<Committee> Find(Expression<Func<Committee, bool>> predicate){
            return _context.Committees.Where(predicate);
        }

        public IQueryable<Committee> FindAll(){
            return _context.Committees;
        }

        public void Remove(Committee committee){
            var committeeToRemove = _context.Committees.Find(committee.Id);
            _context.Committees.Remove(committeeToRemove);
        }

        public void Update(Committee committee){
            var oldentity = _context.Committees.Find(committee.Id);
            _context.Entry(oldentity).CurrentValues.SetValues(committee);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
