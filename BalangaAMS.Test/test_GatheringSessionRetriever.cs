using System.Linq;
using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using System;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_GatheringSessionRetriever
    {
        [Test]
        public void test_GetGatheringSessionThatBrethrenAttendedForTheWholeMonthOf()
        {
            UnityBootstrapper.Configure();

            int year = 2013;

            var brethren =
              UnityBootstrapper.Container.Resolve<BrethrenManager>()
                               .FindBrethren(b => b.ChurchId == "00610865")
                               .FirstOrDefault();



            var gatheringsessionretriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var GsInAMonth = gatheringsessionretriever
                .GetAllGatheringsForMonthOf((MonthofYear) DateTime.Now.Month, year);
            var GsThatBrethrenAttended = gatheringsessionretriever
                .GetGatheringsThatBrethrenAttendedForTheMonthOf(brethren.Id, MonthofYear.June, year);

            var GsThatBrethrenDidntAttended = gatheringsessionretriever
                .GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(brethren.Id, MonthofYear.June, year);
        }

        [Test]
        public void test_GetGatheringSessionForTheMonthOf()
        {
            UnityBootstrapper.Configure();

            int year = 2013;

            var gatheringSessionRetriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringsession = gatheringSessionRetriever
                .GetAllGatheringsForMonthOf(MonthofYear.May, year);

        }

        [Test]
        public void test_linq_GetGatheringSessionThatBrethrenAttendedForTheWholeMonth()
        {
            UnityBootstrapper.Configure();
            using (var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>())
            {

                var brethren = repo.BrethrenBasics.Find(b => b.ChurchId == "00610865").FirstOrDefault();

                MonthofYear monthofYear = MonthofYear.June;
                var gatheringsessionattended = repo.AttendanceLogs.Find(a => a.BrethrenId == brethren.Id
                    && (MonthofYear)a.GatheringSession.Date.Month == monthofYear)
                                    .Select(at => at.GatheringSession).ToList();
            }
        }

        [Test]
        public void test_GetGatheringSessionThatBrehtrenDidntAttendForTheMonthOf()
        {
            UnityBootstrapper.Configure();
            var gatheringsessionretriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "00610865")
                                 .FirstOrDefault();

            MonthofYear monthofYear = MonthofYear.June;
            int year = 2013;

            var gatheringSessionAttended = gatheringsessionretriever
                .GetGatheringsThatBrethrenAttendedForTheMonthOf(brethren.Id, monthofYear, year);

            var gatheringSessionNotAttended = gatheringsessionretriever
                .GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(brethren.Id, monthofYear, year);
        }

        [Test]
        public void test_GetGatheringSessionThatBrethrenDidntAttendForLast12Session()
        {
            UnityBootstrapper.Configure();

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "00610865")
                                 .FirstOrDefault();

            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringSessionNotAttended =
                retriever.GetGatheringsThatBrethrenDidntAttendForLast12Session(brethren.Id);

        }

        [Test]
        public void test_linq_getLast12GatheringSession()
        {
            UnityBootstrapper.Configure();
            var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>();

            var brethren = repo.BrethrenBasics.Find(b => b.ChurchId == "00610865").FirstOrDefault();

            var lastgatheringsession = repo.GatheringSessions.FindAll()
                .OrderByDescending(g => g.Date).Take(12).ToList();

            var gatheringsessionattended = lastgatheringsession
                .Where(l => l.AttendanceLogs.Any(a => a.BrethrenId == brethren.Id)).ToList();

            var gatheringSissionThatNoAttended =
                lastgatheringsession.Except(gatheringsessionattended).ToList();
        }

        [Test]
        public void test_two_methods()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "00610865")
                                 .FirstOrDefault();

            var attendedInMonth =
                retriever.GetGatheringsThatBrethrenAttendedForTheMonthOf(brethren.Id, MonthofYear.May, 2013);
            var notAttendedInMonth =
                retriever.GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(brethren.Id, MonthofYear.May, 2013);
            

            var attendedInLast12Session =
                retriever.GetGatheringsThatBrethrenAttendedForLast12Session(brethren.Id);
            var notAttendedInLast12Session =
                retriever.GetGatheringsThatBrethrenDidntAttendForLast12Session(brethren.Id);

            Assert.AreEqual(notAttendedInMonth, notAttendedInLast12Session);
        }

        [Test]
        public void test_Count()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            long brethrenId = 49;
            MonthofYear monthofYear = MonthofYear.June;
            int year = 2013;
            
            var totalGatherings = retriever.CountAllGatheringsInAMonth(monthofYear, year);
            var totalStartedGatherings = retriever.CountStartedGatheringsInAMonth(monthofYear, year);
            var totalAttendance = retriever.CountBrethrenAttendancedInAMonth(brethrenId, monthofYear, year);
            var totalAbsent = retriever.CountBrethrenAbsentInAMonth(brethrenId, monthofYear, year);
        }

        [Test]
        public void test_IsAttendedThisGathering()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            bool isAttended = retriever.IsAttendedThisGathering(15, 49);
        }
    }
}
