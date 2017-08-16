using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IBrethrenManager
    {
        void AddBrethren(BrethrenBasic brethrenBasic);
        void Updatebrethren(BrethrenBasic brethrenBasic);
        BrethrenBasic GetBrethrenbyId(long id);
        List<BrethrenBasic> FindBrethren(Expression<Func<BrethrenBasic, bool>> predicate);
        List<BrethrenBasic> GetAllBrethren();
        void DeleteBrethren(BrethrenBasic brethrenBasic);
        bool IsNewlyBaptised(BrethrenBasic brethren, int daysToConsiderNewlyBaptised, DateTime dateNow);
    }
}
