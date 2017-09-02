using System;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.WPF.View.DTO
{
    public class BrethrenListViewCheckDTO
    {
        public BrethrenBasic Brethren { get; set; }
        public DateTime LogTime { get; set; }
        public bool HasTime { get; set; }

        public string DateTimeString{
            get{
                if (HasTime){
                    return LogTime.ToString("g");
                }
                return LogTime.ToString("d");
            }
        }

        public bool IsOtherLocal { get; set; }
        public bool IsLate { get; set; }
    }
}