using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using Remotion.Logging;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class OtherLocalLogManager : IOtherLocalManager
    {
        private readonly IChurchGatheringRepository _gatheringRepo;
        private readonly IOtherLocalLogRepository _otherRepo;
        private bool _isSuccessfulLogin;
        private string _message;

        public OtherLocalLogManager(IOtherLocalLogRepository otherRepo, IChurchGatheringRepository gatheringRepo){
            _otherRepo = otherRepo;
            _gatheringRepo = gatheringRepo;         
        }

        public void LogAttendance(OtherLocalLog g, long gatheringId)
        {
            try{
                var gs = _gatheringRepo.Find(s => s.Id == gatheringId).FirstOrDefault();

                if (gs != null){
                    gs.OtherLocalLogs.Add(g);
                    _gatheringRepo.Commit();
                    _isSuccessfulLogin = true;
                    _message = "Success Logging";
                }
                else
                {
                    throw new Exception("gathering Session is null!");
                }

            }
            catch (Exception ex){
                _isSuccessfulLogin = false;
                _message = ex.Message;
            }
        }

        public List<OtherLocalLog> GetLogsByChurchGathering(GatheringSession g){
            List<OtherLocalLog> logs = _otherRepo.FindAll().Where(l => l.GatheringSession.Id == g.Id).ToList();
            return logs;
        }

        public bool IsAlreadyLogin(string churchId, long gatheringId){
            var log =
                _otherRepo.FindAll().FirstOrDefault(l => l.GatheringSession.Id == gatheringId && l.ChurchId == churchId);
            return log != null;
        }

        public bool IsSuccessfulLogin(){
            return _isSuccessfulLogin;
        }

        public string Statusmessage(){
            return _message;
        }

        public void RemoveAttendanceLog(string churchId, long sessionId){
            var logs = _otherRepo.Find(a => a.GatheringSession.Id == sessionId && a.ChurchId == churchId)
                .ToList();
            foreach (var log in logs){
                _otherRepo.Remove(log);
                _otherRepo.Commit();
            }
        }
    }
}
