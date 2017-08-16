using System.ComponentModel.DataAnnotations;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Core.Domain
{
    public class BrethrenFingerPrint : IEntity
    {
        public long Id { get; set; }
        [Required]
        public byte[] Template { get; set; }
    }
}
