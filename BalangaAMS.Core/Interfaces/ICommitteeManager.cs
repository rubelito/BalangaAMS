using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface ICommitteeManager
    {
        void AddCommittee(Committee committee);
        void RemoveCommittee(Committee committee);
        void UpdateCommitee(Committee committee);
        Committee GetCommitteeById(long id);
        List<Committee> GetAllCommittee();
        void AddBrethrenToCommittee(long brethrenid, long committeeid);
        void RemoveBrethrenFromCommittee(long brethrenid, long committeeid);
        List<BrethrenBasic> GetBrethrenInThisCommittee(long committeId);
    }
}