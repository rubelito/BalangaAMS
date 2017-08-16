using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class BrethrenManager : IBrethrenManager
    {
        private readonly IBrethrenRepository _brethrenRepository;

        public BrethrenManager(IBrethrenRepository repo){
            _brethrenRepository = repo;
        }

        public void AddBrethren(BrethrenBasic brethrenBasic){
            if (string.IsNullOrWhiteSpace(brethrenBasic.Name)) throw new ArgumentException("You must supply Name");
            
            _brethrenRepository.Add(brethrenBasic);
            _brethrenRepository.Commit();
        }

        public void Updatebrethren(BrethrenBasic brethrenBasic){
            if (string.IsNullOrWhiteSpace(brethrenBasic.Name)) throw new ArgumentException("You must supply Name");

            _brethrenRepository.Update(brethrenBasic);
            _brethrenRepository.Commit();
        }

        public void UpdatebrethrenWithLastStatusUpdated(BrethrenBasic brethrenBasic){
            brethrenBasic.LastStatusUpdate = DateTime.Now;
            _brethrenRepository.Update(brethrenBasic);
            _brethrenRepository.Commit();
        }

        public BrethrenBasic GetBrethrenbyId(long id){
            return _brethrenRepository.Find(b => b.Id == id).FirstOrDefault();
        }

        public List<BrethrenBasic> FindBrethren(Expression<Func<BrethrenBasic, bool>> predicate){
            return _brethrenRepository.Find(predicate).ToList();
        }

        public List<BrethrenBasic> GetAllBrethren(){
            return _brethrenRepository.FindAll().ToList();
        }

        public void DeleteBrethren(BrethrenBasic brethrenBasic){
            _brethrenRepository.Remove(brethrenBasic);
            _brethrenRepository.Commit();
        }

        public bool IsNewlyBaptised(BrethrenBasic brethren, int daysToConsiderNewlyBaptised, DateTime dateNow){
            if (brethren.BrethrenFull.DateofBaptism.HasValue)
                return ((dateNow - brethren.BrethrenFull.DateofBaptism.Value).TotalDays <= daysToConsiderNewlyBaptised);
            return false;
        }
    }
}
