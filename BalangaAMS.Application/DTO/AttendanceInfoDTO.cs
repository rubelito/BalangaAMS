using System;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.DTO
{
    public class AttendanceInfoDTO
    {
        public BrethrenBasic Brethren { get; set; }
        public String GroupName { get; set; }
        public AttendanceDayInfo AttendaceDayInfo { get; set; }
    }
}
