using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.WPF.View.Schedule
{
    public class AppointmentGenerator
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly DateTime _dateNow;

        public AppointmentGenerator(IChurchGatheringRetriever sessionRetriever, DateTime dateNow)
        {
            _sessionRetriever = sessionRetriever;
            _dateNow = dateNow;
        }

        public ObservableCollection<Gathering> GetPreviousNowNextGatherings()
        {
            var gatheringsInPreviousMonth = GetPreviousMonthGatherings();
            var gatheringsInNowMonth = GetNowMonthGatherings();
            var gatheringsInNextMonth = GetNextMonthGatherings();

            var allgatheringList = GetCombineAllGatherins(gatheringsInPreviousMonth, gatheringsInNowMonth,
                gatheringsInNextMonth);

            return new ObservableCollection<Gathering>(allgatheringList);
        }

        private List<Gathering> GetPreviousMonthGatherings()
        {
            var previousMonthTime = _dateNow.AddMonths(-1);
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)previousMonthTime.Month,
                previousMonthTime.Year);

            return session.Select(g => new Gathering
            {
                IsStarted = g.IsStarted,
                HasAttendees = _sessionRetriever.HasAttendees(g.Id),
                Start = g.Date,
                End = g.Date.AddHours(1),
                UniqueId = Convert.ToString(g.Id),
                Subject = g.Gatherings.ToString()
            }).ToList();
        }

        private List<Gathering> GetNowMonthGatherings()
        {
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)_dateNow.Month, _dateNow.Year);

            return session.Select(g => new Gathering
            {
                IsStarted = g.IsStarted,
                HasAttendees = _sessionRetriever.HasAttendees(g.Id),
                Start = g.Date,
                End = g.Date.AddHours(1),
                UniqueId = Convert.ToString(g.Id),
                Subject = g.Gatherings.ToString()
            }).ToList();
        }

        private List<Gathering> GetNextMonthGatherings()
        {
            var nextMonthTime = _dateNow.AddMonths(1);
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)nextMonthTime.Month, nextMonthTime.Year);

            return session.Select(g => new Gathering
            {
                IsStarted = g.IsStarted,
                HasAttendees = _sessionRetriever.HasAttendees(g.Id),
                Start = g.Date,
                End = g.Date.AddHours(1),
                UniqueId = Convert.ToString(g.Id),
                Subject = g.Gatherings.ToString()
            }).ToList();
        }

        private List<Gathering> GetCombineAllGatherins(List<Gathering> previousMonth, List<Gathering> nowMonth,
            List<Gathering> nextMonth)
        {
            var allGatherings = new List<Gathering>();
            allGatherings.AddRange(previousMonth);
            allGatherings.AddRange(nowMonth);
            allGatherings.AddRange(nextMonth);

            return allGatherings;
        }
    }
}
