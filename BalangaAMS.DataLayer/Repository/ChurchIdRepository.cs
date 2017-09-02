using System;
using System.Linq;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;

namespace BalangaAMS.DataLayer.Repository
{
    public class ChurchIdRepository : IChurchIdRepository
    {
        private readonly AMSDbContext _context;

        public ChurchIdRepository(IDatabaseType dbType){
            if (dbType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new AMSDbContext(dbType);
        }

        public void Add(Core.Domain.ChurchId brethren){
            _context.ChurchIds.Add(brethren);
        }

        public IQueryable<Core.Domain.ChurchId> Find(System.Linq.Expressions.Expression<Func<Core.Domain.ChurchId, bool>> predicate){
            return _context.ChurchIds.Where(predicate);
        }

        public IQueryable<Core.Domain.ChurchId> FindAll(){
            return _context.ChurchIds;
        }

        public void Remove(Core.Domain.ChurchId brethren)
        {
            var idToRemove = _context.ChurchIds.Find(brethren.Id);
            _context.ChurchIds.Remove(idToRemove);
        }

        public void Update(Core.Domain.ChurchId brethren)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
