using System;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.WPF.View.DTO
{
    public class DisplayMonthYearDTO
    {
        public int Year { get; set; }
        public MonthofYear Month { get; set; }
        public DateTime Date { get; set; }

        public string YearMonth
        {
            get
            {
                return Convert.ToString(Year) + " - " + Month;
            }
        }
    }
}
