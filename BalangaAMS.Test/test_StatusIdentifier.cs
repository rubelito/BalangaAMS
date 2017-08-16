using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.Core.Domain;
using BalangaAMS.DataLayer.Repository;
using System.Linq;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_StatusIdentifier
    {
        [Test]
        public void test_GetAttendanceStatusForThisMonth()
        {
            //"00902206"
            //"00610865"
            //"00905363"
            //"B1500002"
            //"B1500003"
            UnityBootstrapper.Configure();
            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "B1500003")
                                 .FirstOrDefault();
            var statusIdentifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var brethrenStatus = statusIdentifier.GetStatusForMonthOf(brethren.Id, MonthofYear.May, 2013);


            Assert.AreEqual(AttendanceStatus.Active, brethrenStatus);
        }

        [Test]
        public void test_GetStatusForLast12Session()
        {
            UnityBootstrapper.Configure();
            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "B1500165")
                                 .FirstOrDefault();
            var statusIdentifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var brethrenStatus = statusIdentifier.GetStatusForLast12Session(brethren.Id);

            Assert.AreEqual(AttendanceStatus.Active, brethrenStatus);
        }

        [Test]
        public void test_GetStatusForMonth_shoul_be_Inactive()
        {
            UnityBootstrapper.Configure();
            var statusIdentifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var status = statusIdentifier.GetStatusForMonthOf(7, MonthofYear.May, 2013);
            Assert.AreEqual(AttendanceStatus.Inactive, status);
        }

        [Test]
        public void test_brethren_should_be_active_whene_there_is_no_gatherings_in_a_month()
        {
            UnityBootstrapper.Configure();
            var identifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();

            var status = identifier.GetStatusForMonthOf(49, MonthofYear.January, 2013);
            Assert.AreEqual(AttendanceStatus.Active, status);
        }

        [Test]
        public void test_something()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var session = retriever.GetGatheringsThatBrehtrenDidntAttendForTheMonthOf(12, MonthofYear.February, 2012);
        }

        [Test]
        public void test_GetNumberOfAbsentToBeInactive()
        {
            UnityBootstrapper.Configure();
            var identifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var numberOfAbsentToBeInactive = identifier.GetNumberOfAbsentToBeInactive(MonthofYear.May, 2013);
        }

        [Test]
        public void test_GetStatusForMonth_should_be_active()
        {
            UnityBootstrapper.Configure();
            var identifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            long brethrenId = 49;
            MonthofYear monthofYear = MonthofYear.November;

            var totalGatherings = retriever.GetStartedGatheringsForMonthOf(monthofYear, 2013).Count;
            var numberOfAbsent = retriever.CountBrethrenAbsentInAMonth(brethrenId, monthofYear, 2013);
            var numberOfAttendance = retriever.CountBrethrenAttendancedInAMonth(brethrenId, monthofYear, 2013);
            var status = identifier.GetStatusForMonthOf(brethrenId, monthofYear, 2013);
            var numberofAbsentToBeInactive = identifier.GetNumberOfAbsentToBeInactive(monthofYear, 2013);
            var numberofPresentToBeActive = identifier.GetNumberOfPresentTobeActive(monthofYear, 2013);
            
            Console.WriteLine("Total No. of Gatherings : " + totalGatherings);
            Console.WriteLine("No. of Absent : " + numberOfAbsent);
            Console.WriteLine("No. of Present : " + numberOfAttendance);
            Console.WriteLine("Status : " + status);
            Console.WriteLine("No. of Absent to be Inactive : " + numberofAbsentToBeInactive);
            Console.WriteLine("No. of Present to be Active : " + numberofPresentToBeActive);
            Assert.AreEqual(AttendanceStatus.Active, status);
        }

        [Test]
        public void test_GetStatusForLast12Session_should_be_active()
        {
            UnityBootstrapper.Configure();
            var identifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            var last12gatherings = retriever.GetLast12StartedGatherings();
            var attendancelog = last12gatherings.Where(g => g.AttendanceLogs.Any(a => a.BrethrenId == 44)).ToList();
        }

        [Test]
        public void test_IsBaptisedThisMonth()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethren = brethrenManager.GetBrethrenbyId(41);

            bool thisMonth = brethren.BrethrenFull.DateofBaptism.HasValue;
            if (thisMonth)
            {
                thisMonth = brethren.BrethrenFull.DateofBaptism.Value.Year == DateTime.Now.Year &&
                        brethren.BrethrenFull.DateofBaptism.Value.Month == DateTime.Now.Month;
            }
            Assert.IsTrue(thisMonth);
        }

        [Test]
        public void test_IsPresentOnlyThisMonth()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethren = brethrenManager.GetBrethrenbyId(52);

            bool isTrue = brethren.LastStatusUpdate.Year == DateTime.Now.Year &&
                   brethren.LastStatusUpdate.Month == DateTime.Now.Month;

            Assert.IsTrue(isTrue);
        }
    }
}
