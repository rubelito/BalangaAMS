using System;
using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service.LoggingAttendance
{
    public class LogAuthenticaterByChurchId : ILogAuthenticaterByChurchId
    {
        private readonly IBrethrenRepository _brethrenRepository;
        private string _churchId;
        private string _messagestatus;
        private BrethrenBasic _brethrenBasic;

        public LogAuthenticaterByChurchId(IBrethrenRepository brethrenRepository){
            _brethrenRepository = brethrenRepository;
        }

        public BrethrenBasic Authenticate(string churchId){
            _churchId = churchId;
            if (churchId == null){
                _messagestatus = "churchId cannot be null";
                throw new Exception("churchId cannot be null");
            }
            return Authenticatebrethren();
        }

        private BrethrenBasic Authenticatebrethren(){
            try{
                _brethrenBasic = _brethrenRepository.Find(b => b.ChurchId == _churchId).FirstOrDefault();
                if (_brethrenBasic != null){
                    _messagestatus = "success";
                }
                else{
                    _messagestatus = "Unknown ID";
                }
            }
            catch (Exception exception){
                _messagestatus = exception.Message;
            }

            return _brethrenBasic;
        }

        public string Statusmessage(){
            return _messagestatus;
        }
    }
}