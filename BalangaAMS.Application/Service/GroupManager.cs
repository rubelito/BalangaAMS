using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using System.Collections.Generic;
using BalangaAMS.Core.Repository;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class GroupManager : IReturnStatus, IGroupManager
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IBrethrenRepository _brethrenRepository;
        private string _statusmessage = "No message";

        public GroupManager(IGroupRepository repo, IBrethrenRepository brethrenRepository){
            _groupRepository = repo;
            _brethrenRepository = brethrenRepository;
        }

        public void Addgroup(Group group){
            bool isExist = _groupRepository.FindAll().Any(g => g.GroupName == group.GroupName);

            if (isExist != true){
                _groupRepository.Add(group);
                _groupRepository.Commit();
                _statusmessage = "success adding group";
            }
            else{
                _statusmessage = "failed adding group: group already exist";
            }

        }

        public void Removegroup(Group group){
            var groupToRemove = GetGroupbyId(group.Id);
            _groupRepository.Remove(groupToRemove);
            _groupRepository.Commit();
        }

        public void Updategroup(Group group){
            _groupRepository.Update(group);
            _groupRepository.Commit();
        }

        public List<Group> Getallgroup(){
            return _groupRepository.FindAll().ToList();
        }

        public Group GetGroupbyId(long id){
            return _groupRepository.Find(g => g.Id == id).FirstOrDefault();
        }

        public void AddBrethrenToAGroup(long brethrenid, long groupid){
            var group = _groupRepository.Find(g => g.Id == groupid).FirstOrDefault();
            var brethren = _brethrenRepository.Find(b => b.Id == brethrenid).FirstOrDefault();
            _groupRepository.AddBrethrenToGroup(group, brethren.Id);
            _groupRepository.Commit();
        }

        public string Statusmessage(){
            return _statusmessage;
        }

        public void RemoveBrethrenToAGroup(long churchid){
            var brethren = _brethrenRepository.Find(b => b.Id == churchid).FirstOrDefault();
            var group = _groupRepository.Find(g => g.Id == brethren.Group.Id).FirstOrDefault();
            _groupRepository.RemoveBrethrenToGroup(group, brethren.Id);
            _groupRepository.Commit();
        }

        public List<BrethrenBasic> GetBrethrenWithInGroup(long groupid){
            return _brethrenRepository.Find(b => b.Group.Id == groupid).OrderBy(b => b.Name).ToList();
        }

        public List<BrethrenBasic> GetBrethrenWithNoGroup(){
            return _brethrenRepository.Find(b => b.Group == null).OrderBy(b => b.Name).ToList();
        }
    }
}
