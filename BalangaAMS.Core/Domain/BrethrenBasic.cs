using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using System;

namespace BalangaAMS.Core.Domain
{
    public class BrethrenBasic : BindableBase ,IEntity
    {
        private List<Committee> _committees; 
        public BrethrenBasic()
        {
            _committees = new List<Committee>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        private string _churchId;
        public string ChurchId { get { return _churchId; } set {SetProperty(ref _churchId, value);} }

        private string _name;
        [Required]
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        private LocalStatus _localStatus;
        public LocalStatus LocalStatus { get { return _localStatus; } set { SetProperty(ref _localStatus, value); } }

        private DateTime _laststatusupdated;
        public DateTime LastStatusUpdate 
        { 
            get { return _laststatusupdated; } 
            set { SetProperty(ref _laststatusupdated, value); }
        }

        public virtual BrethrenFull BrethrenFull { get; set; }

        private Group _group;
        public virtual Group Group { get { return _group; } set { SetProperty(ref _group, value); } }
        public virtual List<Committee> Committees { get { return _committees; } set { _committees = value; } }
        public virtual BrethrenFingerPrint FingerPrint { get; set; }
    }
}