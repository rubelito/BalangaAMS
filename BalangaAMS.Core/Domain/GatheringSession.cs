using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalangaAMS.Core.Domain
{
    public class GatheringSession : IEntity
    {
        private List<AttendanceLog> _attendanceLogs;

        public GatheringSession()
        {
            _attendanceLogs = new List<AttendanceLog>();   
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public Gatherings Gatherings { get; set; }
        public bool IsStarted { get; set; }

        public virtual List<AttendanceLog> AttendanceLogs
        {
            get { return _attendanceLogs; }
            set { _attendanceLogs = value; }
        } 
    }
}
