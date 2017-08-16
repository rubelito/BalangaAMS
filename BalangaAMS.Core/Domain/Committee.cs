using BalangaAMS.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalangaAMS.Core.Domain
{
    public class Committee : IEntity
    {
        private List<BrethrenBasic> _brethrenBasics;
        public Committee()
        {
            _brethrenBasics = new List<BrethrenBasic>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual List<BrethrenBasic> BrethrenBasics { get { return _brethrenBasics; } set { _brethrenBasics = value; } } 
    }
}