using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IOtherLocalManager : IReturnStatus
    {
        void LogAttendance(OtherLocalLog g, long gatheringId);
        List<OtherLocalLog> GetLogsByChurchGathering(GatheringSession g);
        void RemoveAttendanceLog(string churchId, long sessionId);
        bool IsAlreadyLogin(string churchId, long gatheringId);
        bool IsSuccessfulLogin();
    }
}
