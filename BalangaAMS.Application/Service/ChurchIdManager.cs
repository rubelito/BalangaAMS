using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class ChurchIdManager : IChurchIdManager
    {
        private readonly IChurchIdRepository _repo;

        public ChurchIdManager(IChurchIdRepository repo){
            _repo = repo;
        }

        public void AddChurchId(Core.Domain.ChurchId cId){
            if (!IsChurchIdAlreadyExist(cId.Code)){
                _repo.Add(cId);
                _repo.Commit();
            }
        }

        public bool IsChurchIdAlreadyExist(string churchId){
            var a = _repo.FindAll().FirstOrDefault(c => c.Code == churchId);
            return a != null;
        }

        public List<Core.Domain.ChurchId> FindChurchIds(System.Linq.Expressions.Expression<Func<Core.Domain.ChurchId, bool>> predicate)
        {
            return _repo.Find(predicate).ToList();
        }

        public List<Core.Domain.ChurchId> GetAllChurchIds()
        {
            return _repo.FindAll().ToList();
        }
    }
}
