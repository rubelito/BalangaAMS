using System;
using BalangaAMS.Core.Domain.Enum;

namespace BalangaAMS.ApplicationLayer.DTO
{
    public class AttendanceDayInfo
    {
        public DayAttendanceStatus DayAttendanceStatus { get; set; }
        public DateTime DateAndTime { get; set; }

        public string DateAndTimeStr{
            get{
                if (IsDateHasHourAndMinute(DateAndTime))
                    return DateAndTime.ToString("MM/dd/yyyy");
                return DateAndTime.ToString("g");
            }
        }

        public bool IsAttended { get; set; }

        private bool IsDateHasHourAndMinute(DateTime date){
            return date.Hour == 0 && date.Minute == 0;
        }
    }
}
