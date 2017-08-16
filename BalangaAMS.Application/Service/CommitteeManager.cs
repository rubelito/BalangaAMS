using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class CommitteeManager : IReturnStatus, ICommitteeManager
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IBrethrenRepository _brethrenRepository;
        private string _statusmessage = "No message";

        public CommitteeManager(ICommitteeRepository committeeRepository, IBrethrenRepository brethrenRepository){
            _committeeRepository = committeeRepository;
            _brethrenRepository = brethrenRepository;
        }

        public void AddCommittee(Committee committee){
            bool isExist = _committeeRepository.FindAll().Any(c => c.Name == committee.Name);

            if (isExist != true){
                try{
                    _committeeRepository.Add(committee);
                    _committeeRepository.Commit();
                    _statusmessage = "success adding committee";
                }
                catch (Exception exception){
                    _statusmessage = exception.Message;
                }
            }
            else{
                _statusmessage = "failed adding committee: committee already exist";
            }

        }

        public void RemoveCommittee(Committee committee){
            try{
                _committeeRepository.Remove(committee);
                _committeeRepository.Commit();
                _statusmessage = "success removing committee";
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
            }
        }

        public void UpdateCommitee(Committee committee){
            try{
                _committeeRepository.Update(committee);
                _committeeRepository.Commit();
                _statusmessage = "success updating commitee";
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
            }
        }

        public Committee GetCommitteeById(long id){
            return _committeeRepository.Find(c => c.Id == id).FirstOrDefault();
        }

        public List<Committee> GetAllCommittee(){
            return _committeeRepository.FindAll().ToList();
        }

        public void AddBrethrenToCommittee(long brethrenid, long committeeid){
            var committee = _committeeRepository.Find(c => c.Id == committeeid).FirstOrDefault();
            var brethren = _brethrenRepository.Find(b => b.Id == brethrenid).FirstOrDefault();
            if (!committee.BrethrenBasics.Exists(b => b == brethren)){
                committee.BrethrenBasics.Add(brethren);
                _committeeRepository.Commit();
            }
        }

        public void RemoveBrethrenFromCommittee(long brethrenid, long committeeid){
            var committee = _committeeRepository.Find(c => c.Id == committeeid).FirstOrDefault();
            var brethren = _brethrenRepository.Find(b => b.Id == brethrenid).FirstOrDefault();
            committee.BrethrenBasics.Remove(brethren);
            _committeeRepository.Commit();
        }

        public List<BrethrenBasic> GetBrethrenInThisCommittee(long committeId){
            return _brethrenRepository.Find(b => b.Committees.Any(c => c.Id == committeId)).ToList();
        }

        public string Statusmessage(){
            return _statusmessage;
        }
    }
}
