using System.Collections.Generic;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.WPF.View.DTO
{
    public class DailyAttendanceDTO
    {
        public List<AttendanceInfoDTO> AttendanceInfoList { get; set; }
        public GatheringSession SelectedSession { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
    }
}
