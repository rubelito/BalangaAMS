using System.Collections.ObjectModel;
using BalangaAMS.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalangaAMS.Core.Domain
{
    public class Group :BindableBase, IEntity
    {
        private ObservableCollection <BrethrenBasic> _brethrenBasics; 
        public Group()
        {
            _brethrenBasics = new ObservableCollection<BrethrenBasic>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        private string _groupname;
        public string GroupName { get { return _groupname; } set { SetProperty(ref _groupname, value); } }
        public virtual ObservableCollection<BrethrenBasic> Brethren { get { return _brethrenBasics; } set { SetProperty(ref _brethrenBasics, value); } }
    }
}