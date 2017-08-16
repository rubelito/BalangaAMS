using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IGroupManager
    {
        void Addgroup(Group group);
        void Removegroup(Group group);
        void Updategroup(Group group);
        List<Group> Getallgroup();
        Group GetGroupbyId(long id);
        void AddBrethrenToAGroup(long brethrenid, long groupid);
        void RemoveBrethrenToAGroup(long churchid);
        List<BrethrenBasic> GetBrethrenWithInGroup(long groupid);
        List<BrethrenBasic> GetBrethrenWithNoGroup();
    }
}