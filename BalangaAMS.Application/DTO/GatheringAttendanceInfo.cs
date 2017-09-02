using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.DTO
{
    public class GatheringAttendanceInfo
    {
        public GatheringAttendanceInfo(){
            Attendees = new List<BrethrenBasic>();
        }

        public GatheringSession Gathering { get; set; }
        public List<BrethrenBasic> Attendees { get; set; }
        public List<string> OtherLocalChurchIds { get; set; } 
    }
}
