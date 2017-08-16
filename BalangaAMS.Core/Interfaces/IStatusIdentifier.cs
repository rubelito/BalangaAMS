using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IStatusIdentifier
    {
        AttendanceStatus GetStatusForMonthOf(long brethrenId, MonthofYear monthofYear ,int year);
        AttendanceStatus GetStatusForLast12Session(long brethrenId);
        int GetNumberOfAbsentToBeInactive(MonthofYear monthofYear, int year);
        int GetNumberOfPresentTobeActive(MonthofYear monthofYear, int year);
    }
}