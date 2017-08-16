using System;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service.GatheringSessionManager
{
    public class ChurchGatheringManager: IReturnStatus, IChurchGatheringManager
    {
        private readonly IChurchGatheringRepository _repo;
        private string _statusmessage = "No status";
        private GatheringSession _gatheringSession;
        private bool _isRemovingSuccessful;

        public ChurchGatheringManager(IChurchGatheringRepository repo){
            _repo = repo;
        }

        public void CreateGathering(Gatherings gatherings, DateTime datestarted){
            try{
                _gatheringSession = new GatheringSession{
                    Date = datestarted,
                    Gatherings = gatherings,
                    IsStarted = false
                };
                _repo.Add(_gatheringSession);
                _repo.Commit();
                _statusmessage = "Success Creating Session";
            }
            catch (Exception exception){
                _statusmessage = exception.Message;
            }
        }

        public GatheringSession ReturnNewlyCreatedGathering(){
            return _gatheringSession;
        }

        public string Statusmessage(){
            return _statusmessage;
        }

        public void UpdateGathering(GatheringSession gathering){
            _repo.Update(gathering);
            _repo.Commit();
        }

        public void RemoveGathering(GatheringSession gathering){
            try{
                _repo.Remove(gathering);
                _repo.Commit();
                _isRemovingSuccessful = true;
            }
            catch (Exception){
                _isRemovingSuccessful = false;
            }
        }

        public bool IsRemovingSuccessful(){
            return _isRemovingSuccessful;
        }
    }
}