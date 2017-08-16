using BalangaAMS.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace BalangaAMS.Core.Domain
{
    public class AttendanceLog: IEntity
    {
        public long Id { get; set; }
        [Required]
        public long BrethrenId { get; set; }
        public DateTime DateTime { get; set; }
        [MaxLength(75, ErrorMessageResourceName = "AttendanceLog.WorkersAssigned: only maximum of 75 characters")]
        public string WorkersAssigned { get; set; }
        public bool IsLate { get; set; }
   
        [Required]
        public virtual GatheringSession GatheringSession { get; set; }
    }
}
