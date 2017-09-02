using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IChurchIdManager
    {
        void AddChurchId(ChurchId cId);
        List<ChurchId> FindChurchIds(Expression<Func<ChurchId, bool>> predicate);
        List<ChurchId> GetAllChurchIds();
    }
}
