using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IAttendanceLogger: IReturnStatus
    {
        void Logbrethren(long gatheringSessionId, AttendanceLog attendance);
        bool IsSuccessfulLogging();
    }
}
