using System.Collections.Generic;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;

namespace BalangaAMS.WPF.View.DTO
{
    public class IndividualMonthlyInfoDTO
    {
        public List<BrethrenBasic> Brethrens { get; set; }
        public string DestinationPath { get; set; }
        public MonthofYear MonthofYear { get; set; }
        public int Year { get; set; }
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
    }
}
