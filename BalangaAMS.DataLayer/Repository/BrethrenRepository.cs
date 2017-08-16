using System;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.DataLayer.Repository
{
    public class BrethrenRepository : IBrethrenRepository
    {
        private readonly AMSDbContext _context;

        public BrethrenRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(databaseType);
        }

        public void Add(BrethrenBasic brethren){
            _context.BrethrenBasics.Add(brethren);
            _context.BrethrenFulls.Add(brethren.BrethrenFull);
        }

        public IQueryable<BrethrenBasic> Find(Expression<Func<BrethrenBasic, bool>> predicate){
            return _context.BrethrenBasics.Where(predicate);
        }

        public IQueryable<BrethrenBasic> FindAll(){
            return _context.BrethrenBasics;
        }

        public void Remove(BrethrenBasic brethren){
            var oldbrethren = _context.BrethrenBasics.Find(brethren.Id);
            var oldbrethrenFull = _context.BrethrenFulls.Find(oldbrethren.BrethrenFull.Id);
            _context.BrethrenFulls.Remove(oldbrethrenFull);
            _context.BrethrenBasics.Remove(oldbrethren);
        }

        public void Update(BrethrenBasic brethren){
            var oldbrethren = _context.BrethrenBasics.Find(brethren.Id);
            _context.Entry(oldbrethren).CurrentValues.SetValues(brethren);
            var oldBrethrenFull = _context.BrethrenFulls.Find(brethren.BrethrenFull.Id);
            _context.Entry(oldBrethrenFull).CurrentValues.SetValues(brethren.BrethrenFull);
        }

        public void Commit(){
            _context.SaveChanges();
        }
    }
}
