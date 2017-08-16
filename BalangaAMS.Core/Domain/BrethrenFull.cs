using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using System;

namespace BalangaAMS.Core.Domain
{
    public class BrethrenFull : BindableBase , IEntity
    {
        public long Id { get; set; }
        public string NickName { get; set; }
        private Nullable<DateTime> _dateOfBaptism;
        public Nullable<DateTime> DateofBaptism { get { return _dateOfBaptism; } set
        {
            SetProperty(ref _dateOfBaptism, value);
        } }     
        public string PlaceofBaptism { get; set; }
        public string Baptizer { get; set; }
        public string Contactno { get; set; }
        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string Town { get; set; }
        public string Province { get; set; }
        public string Language { get; set; }
        private Nullable<DateTime> _dateOfBirth;
        public Nullable<DateTime> DateofBirth { get { return _dateOfBirth; } set 
        {
                SetProperty(ref _dateOfBirth, value);
        } }
        public Gender Gender { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public string Job { get; set; }
        public string Skills { get; set; }
        public string EducationalAttainment { get; set; }
    }
}