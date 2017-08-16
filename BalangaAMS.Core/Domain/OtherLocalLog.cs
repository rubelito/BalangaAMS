using System.ComponentModel.DataAnnotations;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Core.Domain
{
    public class OtherLocalLog : IEntity
    {
        public long Id { get; set; }
        [Required]
        public string ChurchId { get; set; }
        [Required]
        public virtual GatheringSession GatheringSession { get; set; }
        //Just added a comment
    }
}