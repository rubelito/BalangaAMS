using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.WPF.View.DTO;

namespace BalangaAMS.WPF.View.HelperClass
{
    public class YearMonthGetter
    {
        public List<DisplayMonthYearDTO> GetYearMonth(List<GatheringSession> sessionList)
        {
            var yearMonth = new List<DisplayMonthYearDTO>();
            foreach (var session in sessionList)
            {
                var year = session.Date.Year;
                var month = (MonthofYear) session.Date.Month;
                if (!IsYearMonthAdded(yearMonth, year, month))
                {
                    yearMonth.Add(new DisplayMonthYearDTO
                    {
                        Year = year,
                        Month = month,
                        Date = new DateTime(year, (int) month, 1)
                    });
                }
            }
            return yearMonth;
        }

        private bool IsYearMonthAdded(List<DisplayMonthYearDTO> monthYear, int year, MonthofYear monthofYear)
        {
            return monthYear.Any(g => g.Year == year && g.Month == monthofYear );
        }
    }
}
