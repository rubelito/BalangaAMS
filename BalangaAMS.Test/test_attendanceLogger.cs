using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.ApplicationLayer.Service.LoggingAttendance;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using System;
using Microsoft.Practices.Unity;
using BalangaAMS.DataLayer.EntityFramework;
using System.Linq;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_attendanceLogger
    {
        [Test]
        public void test_LogAuthenticaterById()
        {
            UnityBootstrapper.Configure();
            var logauthenticater = UnityBootstrapper.Container.Resolve<LogAuthenticaterByChurchId>();
            var brethren = logauthenticater.Authenticate("00610865");

            Assert.IsNotNull(brethren);
        }

        [Test]
        public void test_BrethrenAttendancelogger()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringsession = retriever.GetGatheringById(8);

            var authenticater = UnityBootstrapper.Container.Resolve<LogAuthenticaterByChurchId>();
            var brethren = authenticater.Authenticate("00610865");

            if (brethren != null)
            {
                var attendancelog = new AttendanceLog()
                    {
                        WorkersAssigned = "Bro. Kaloy", 
                        BrethrenId = brethren.Id, 
                        DateTime = DateTime.Now
                    };

                var logger = UnityBootstrapper.Container.Resolve<BrethrenAttendancelogger>();
                logger.Logbrethren(gatheringsession.Id, attendancelog);
                Assert.IsTrue(logger.IsSuccessfulLogging());
            }
        }

        [Test]
        public void test_manual_adding()
        {
            UnityBootstrapper.Configure();
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                var attendancelog = new AttendanceLog();
                var brethren = repo.BrethrenBasics.Find(b => b.ChurchId == "00610865").FirstOrDefault();
                attendancelog.BrethrenId = brethren.Id;
                attendancelog.DateTime = DateTime.Now;
                attendancelog.WorkersAssigned = "Bro, Kaloy";

                var gatheringSesion = repo.GatheringSessions.Find(g => g.Id == 8).FirstOrDefault();
                gatheringSesion.AttendanceLogs.Add(attendancelog);
                repo.Commit();
            }
        }

        [Test]
        public void adding_enough_dummy_attendancelog_to_make_brethren_active()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringSessionList = retriever.GetAllGatheringsForMonthOf(MonthofYear.June, 2013);
            var logger = UnityBootstrapper.Container.Resolve<BrethrenAttendancelogger>();

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.Id == 197)
                                 .FirstOrDefault();

            for (int i = 7; i <= 13; i++)
            {
                var attendancelog = new AttendanceLog()
                    {
                        WorkersAssigned = "Bro. Kaloy",
                        BrethrenId = brethren.Id,
                        DateTime = DateTime.Now
                    };
                if (i%2 == 0)
                {
                    attendancelog.IsLate = true;
                }
          
                logger.Logbrethren(gatheringSessionList[i].Id,attendancelog);
                Console.WriteLine(logger.Statusmessage());
            }
        }

        [Test]
        public void adding_dummy_attendancelog_to_make_brethren_inactive()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringSessionList = retriever.GetAllGatheringsForMonthOf(MonthofYear.June, 2013);

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.Id == 196)
                                 .FirstOrDefault();

            for (int i = 7; i <= 12; i++)
            {
                var attendancelog = new AttendanceLog()
                {
                    WorkersAssigned = "Bro. Kaloy",
                    BrethrenId = brethren.Id,
                    DateTime = DateTime.Now
                };

                if (i%2 == 0)
                {
                    attendancelog.IsLate = true;
                }

                var logger = UnityBootstrapper.Container.Resolve<BrethrenAttendancelogger>();
                logger.Logbrethren(gatheringSessionList[i].Id, attendancelog);
            }
        }

        [Test]
        public void adding_complete_dummy_attendancelog()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringSessionList = retriever.GetAllGatheringsForMonthOf(MonthofYear.May, 2013);

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "00610865")
                                 .FirstOrDefault();

            for (int i = 0; i <= 13 - 1; i++)
            {
                var attendancelog = new AttendanceLog()
                {
                    WorkersAssigned = "Bro. Kaloy",
                    BrethrenId = brethren.Id,
                    DateTime = DateTime.Now
                };
                if (i % 2 != 0)
                {
                    attendancelog.IsLate = true;
                }

                var logger = UnityBootstrapper.Container.Resolve<BrethrenAttendancelogger>();
                logger.Logbrethren(gatheringSessionList[i].Id, attendancelog);
            }
        }
    }
}
