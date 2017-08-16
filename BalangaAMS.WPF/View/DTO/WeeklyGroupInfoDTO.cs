using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.WPF.View.DTO
{
    public class WeeklyGroupInfoDTO
    {
        public Group Group { get; set; }
        public string DestinationPath { get; set; }
        public List<GatheringSession> GatheringSessions { get; set; }
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
    }
}
