using System.Data;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.Core.Interfaces
{
    public interface IMonthlyReport
    {
        DataTable GenerateBrethrenReport(long brethrenId, MonthofYear monthofYear, int year);
    }
}