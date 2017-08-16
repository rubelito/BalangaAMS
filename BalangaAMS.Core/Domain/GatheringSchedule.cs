
using System;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalangaAMS.Core.Domain
{
    public class GatheringSchedule : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Gatherings Gatherings { get; set; }
        public DayOfWeek Day { get; set; }
        public string Time { get; set; }
        public int MinutesBeforePrayer { get; set; }
    }
}
