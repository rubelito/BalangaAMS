using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Repository
{
    public interface IChurchIdRepository
    {
        void Add(ChurchId brethren);
        IQueryable<ChurchId> Find(Expression<Func<ChurchId, bool>> predicate);
        IQueryable<ChurchId> FindAll();
        void Remove(ChurchId brethren);
        void Update(ChurchId brethren);
        void Commit();
    }
}
