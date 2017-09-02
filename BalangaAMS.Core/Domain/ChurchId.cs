using System.ComponentModel.DataAnnotations;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Core.Domain
{
    public class ChurchId : IEntity
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
