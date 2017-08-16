using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalangaAMS.Test
{
    [TestFixture]
    class Test_AppointmentGenerator
    {
        [Test]
        public void test_GetPreviousNowNextGatherings()
        {
            UnityBootstrapper.Configure();
            var sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var ag = new AppointmentGenerator(sessionRetriever, DateTime.Now.AddMonths(-2));
            var gatheringList = ag.GetPreviousNowNextGatherings();

            Assert.AreNotEqual(0, gatheringList.Count);
        }
    }

    public class AppointmentGenerator
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly DateTime _dateNow;

        public AppointmentGenerator(IChurchGatheringRetriever sessionRetriever, DateTime dateNow)
        {
            _sessionRetriever = sessionRetriever;
            _dateNow = dateNow;
        }

        public List<GatheringSession> GetPreviousNowNextGatherings()
        {
            var gatheringsInPreviousMonth = GetPreviousMonthGatherings();
            var gatheringsInNowMonth = GetNowMonthGatherings();
            var gatheringsInNextMonth = GetNextMonthGatherings();

            var allgatheringList = GetCombineAllGatherins(gatheringsInPreviousMonth, gatheringsInNowMonth,
                gatheringsInNextMonth);

            return new List<GatheringSession>(allgatheringList);
        }

        private List<GatheringSession> GetPreviousMonthGatherings()
        {
            var previousMonthTime = _dateNow.AddMonths(-1);
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)previousMonthTime.Month,
                previousMonthTime.Year);

            return session;
        }

        private List<GatheringSession> GetNowMonthGatherings()
        {
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)_dateNow.Month, _dateNow.Year);

            return session;
        }

        private List<GatheringSession> GetNextMonthGatherings()
        {
            var nextMonthTime = _dateNow.AddMonths(1);
            var session = _sessionRetriever.GetAllGatheringsForMonthOf((MonthofYear)nextMonthTime.Month, nextMonthTime.Year);

            return session;
        }

        private List<GatheringSession> GetCombineAllGatherins(List<GatheringSession> previousMonth, List<GatheringSession> nowMonth,
                List<GatheringSession> nextMonth)
        {
            var allGatherings = new List<GatheringSession>();
            allGatherings.AddRange(previousMonth);
            allGatherings.AddRange(nowMonth);
            allGatherings.AddRange(nextMonth);

            return allGatherings;
        }
    }

}
