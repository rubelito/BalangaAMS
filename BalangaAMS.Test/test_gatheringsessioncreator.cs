using System.Linq;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using System;
using Microsoft.Practices.Unity;
using BalangaAMS.ApplicationLayer.Service;


namespace BalangaAMS.Test
{
    [TestFixture]
    internal class test_gatheringsessioncreator
    {
        [Test]
        public void test_creating_gathering_session()
        {
            UnityBootstrapper.Configure();
            var schedulemanager = UnityBootstrapper.Container.Resolve<ScheduleManager>();
            var gatheringschedule = schedulemanager.GetListofSchedule().Where(s => s.Id == 6).FirstOrDefault();

            var sessioncreator = UnityBootstrapper.Container.Resolve<ChurchGatheringManager>();

            sessioncreator.CreateGathering(gatheringschedule.Gatherings, DateTime.Now);

            var gatheringsession = sessioncreator.ReturnNewlyCreatedGathering();

        }

        [Test]
        public void test_RetrieveLatestDifferentGatheringSessions()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();

            var gatherings = retriever.RetrieveLatestDifferentGatherings();
            Console.WriteLine(retriever.Statusmessage());
            Console.WriteLine(gatherings.Count.ToString());

        }

        [Test]
        public void test_RetrieveGatheringSessionInEntireMonth()
        {
            UnityBootstrapper.Configure();
            var retriever = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();

            int year = 2013;

            var gatherings = retriever.GetAllGatheringsForMonthOf((MonthofYear) DateTime.Now.Month, year);
            Console.WriteLine(gatherings.Count.ToString());
        }

        [Test]
        public void test_adding_13_gatherings_for_April()
        {
            var connection = new EfSQLite("SQLiteDb");
            using (var repo = new AMSUnitofWork(connection))
            {
                var prayerMeeting = repo.GatheringSchedules
                                             .Find(g => g.Id == 2).FirstOrDefault();//Prayer Meeting
                var worshipService = repo.GatheringSchedules
                                             .Find(g => g.Id == 5).FirstOrDefault(); //Worship Service
                var thanksGiving = repo.GatheringSchedules
                                             .Find(g => g.Id == 7).FirstOrDefault(); // Thanks Giving

                var gatheringsessioncreator2 = new ChurchGatheringManager(repo);
                gatheringsessioncreator2.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("April 3, 2013 5:30 PM"));

                var gatheringsessioncreator3 = new ChurchGatheringManager(repo);
                gatheringsessioncreator3.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("April 6, 2013 8:30 AM"));

                var gatheringsessioncreator4 = new ChurchGatheringManager(repo);
                gatheringsessioncreator4.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("April 6, 2013 5:00 PM"));

                var gatheringsessioncreator5 = new ChurchGatheringManager(repo);
                gatheringsessioncreator5.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("April 10, 2013 5:30 PM"));

                var gatheringsessioncreator6 = new ChurchGatheringManager(repo);
                gatheringsessioncreator6.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("April 13, 2013 8:30 AM"));

                var gatheringsessioncreator7 = new ChurchGatheringManager(repo);
                gatheringsessioncreator7.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("April 13, 2013 5:00 PM"));

                var gatheringsessioncreator8 = new ChurchGatheringManager(repo);
                gatheringsessioncreator8.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("April 17, 2013 5:30 PM"));

                var gatheringsessioncreator9 = new ChurchGatheringManager(repo);
                gatheringsessioncreator9.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("April 20, 2013 8:30 AM"));

                var gatheringsessioncreator10 = new ChurchGatheringManager(repo);
                gatheringsessioncreator10.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("April 20, 2013 5:00 PM"));

                var gatheringsessioncreator11 = new ChurchGatheringManager(repo);
                gatheringsessioncreator11.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("April 24, 2013 5:30 PM"));

                var gatheringsessioncreator12 = new ChurchGatheringManager(repo);
                gatheringsessioncreator12.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("April 27, 2013 8:30 AM"));

                var gatheringsessioncreator13 = new ChurchGatheringManager(repo);
                gatheringsessioncreator13.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("April 27, 2013 5:00 PM"));
            }
        }

        [Test]
        public void test_adding_13_gatherings_for_May()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                //var gatheringschedule1 = repo.GatheringSchedules
                  //                           .Find(g => g.Id == 1).FirstOrDefault();
                var gatheringschedule2 = repo.GatheringSchedules
                                             .Find(g => g.Id == 2).FirstOrDefault();
                var gatheringschedule3 = repo.GatheringSchedules
                                             .Find(g => g.Id == 5).FirstOrDefault();
                var gatheringschedule4 = repo.GatheringSchedules
                                             .Find(g => g.Id == 7).FirstOrDefault();
                var gatheringschedule5 = repo.GatheringSchedules
                                             .Find(g => g.Id == 2).FirstOrDefault();
                var gatheringschedule6 = repo.GatheringSchedules
                                             .Find(g => g.Id == 5).FirstOrDefault();
                var gatheringschedule7 = repo.GatheringSchedules
                                             .Find(g => g.Id == 7).FirstOrDefault();
                var gatheringschedule8 = repo.GatheringSchedules
                                             .Find(g => g.Id == 2).FirstOrDefault();
                var gatheringschedule9 = repo.GatheringSchedules
                                             .Find(g => g.Id == 5).FirstOrDefault();
                var gatheringschedule10 = repo.GatheringSchedules
                                              .Find(g => g.Id == 7).FirstOrDefault();
                var gatheringschedule11 = repo.GatheringSchedules
                                              .Find(g => g.Id == 2).FirstOrDefault();
                var gatheringschedule12 = repo.GatheringSchedules
                                              .Find(g => g.Id == 5).FirstOrDefault();

             //   var gatheringsessioncreator1 = new GatheringSessionCreator(repo);
               // gatheringsessioncreator1.CreateSession(gatheringschedule1.Gatherings, DateTime.Now);

                var gatheringsessioncreator2 = new ChurchGatheringManager(repo);
                gatheringsessioncreator2.CreateGathering(gatheringschedule2.Gatherings, Convert.ToDateTime("May 1, 2013 5:30 PM"));

                var gatheringsessioncreator3 = new ChurchGatheringManager(repo);
                gatheringsessioncreator3.CreateGathering(gatheringschedule3.Gatherings, Convert.ToDateTime("May 4, 2013 8:30 AM"));

                var gatheringsessioncreator4 = new ChurchGatheringManager(repo);
                gatheringsessioncreator4.CreateGathering(gatheringschedule4.Gatherings, Convert.ToDateTime("May 4, 2013 5:30 PM"));

                var gatheringsessioncreator5 = new ChurchGatheringManager(repo);
                gatheringsessioncreator5.CreateGathering(gatheringschedule5.Gatherings, Convert.ToDateTime("May 8, 2013 5:30 PM"));

                var gatheringsessioncreator6 = new ChurchGatheringManager(repo);
                gatheringsessioncreator6.CreateGathering(gatheringschedule6.Gatherings, Convert.ToDateTime("May 11, 2013 8:30 AM"));

                var gatheringsessioncreator7 = new ChurchGatheringManager(repo);
                gatheringsessioncreator7.CreateGathering(gatheringschedule7.Gatherings, Convert.ToDateTime("May 11, 2013 5:30 PM"));

                var gatheringsessioncreator8 = new ChurchGatheringManager(repo);
                gatheringsessioncreator8.CreateGathering(gatheringschedule8.Gatherings, Convert.ToDateTime("May 15, 2013 5:30 PM"));

                var gatheringsessioncreator9 = new ChurchGatheringManager(repo);
                gatheringsessioncreator9.CreateGathering(gatheringschedule9.Gatherings, Convert.ToDateTime("May 18, 2013 8:30 AM"));

                var gatheringsessioncreator10 = new ChurchGatheringManager(repo);
                gatheringsessioncreator10.CreateGathering(gatheringschedule10.Gatherings, Convert.ToDateTime("May 18, 2013 5:30 PM"));

                var gatheringsessioncreator11 = new ChurchGatheringManager(repo);
                gatheringsessioncreator11.CreateGathering(gatheringschedule11.Gatherings, Convert.ToDateTime("May 22, 2013 5:30 PM"));

                var gatheringsessioncreator12 = new ChurchGatheringManager(repo);
                gatheringsessioncreator12.CreateGathering(gatheringschedule12.Gatherings, Convert.ToDateTime("May 25, 2013 8:30 AM"));

                var gatheringsessioncreator13 = new ChurchGatheringManager(repo);
                gatheringsessioncreator12.CreateGathering(gatheringschedule10.Gatherings, Convert.ToDateTime("May 25, 2013 5:30 PM"));

                var gatheringsessioncreator14 = new ChurchGatheringManager(repo);
                gatheringsessioncreator12.CreateGathering(gatheringschedule8.Gatherings, Convert.ToDateTime("May 29, 2013 5:30 PM"));

            }
        }

        [Test]
        public void test_adding_13_gatherings_for_June()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                //var gatheringschedule1 = repo.GatheringSchedules
                //                           .Find(g => g.Id == 1).FirstOrDefault();
                var prayerMeeting = repo.GatheringSchedules
                                             .Find(g => g.Gatherings == Gatherings.Prayer_Meeting).FirstOrDefault();//Prayer Meeting
                var worshipService = repo.GatheringSchedules
                                             .Find(g => g.Gatherings == Gatherings.Worship_Service).FirstOrDefault(); //Worship Service
                var thanksGiving = repo.GatheringSchedules
                                             .Find(g => g.Gatherings == Gatherings.Thanks_Giving).FirstOrDefault(); // Thanks Giving

                //   var gatheringsessioncreator1 = new GatheringSessionCreator(repo);
                // gatheringsessioncreator1.CreateSession(gatheringschedule1.Gatherings, DateTime.Now);

                var gatheringsessioncreator2 = new ChurchGatheringManager(repo);
                gatheringsessioncreator2.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("June 1, 2013 8:30 AM"));

                var gatheringsessioncreator3 = new ChurchGatheringManager(repo);
                gatheringsessioncreator3.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("June 1, 2013 5:00 PM"));

                var gatheringsessioncreator4 = new ChurchGatheringManager(repo);
                gatheringsessioncreator4.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("June 5, 2013 5:30 PM"));

                var gatheringsessioncreator5 = new ChurchGatheringManager(repo);
                gatheringsessioncreator5.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("June 8, 2013 8:30 AM"));

                var gatheringsessioncreator6 = new ChurchGatheringManager(repo);
                gatheringsessioncreator6.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("June 8, 2013 5:00 PM"));

                var gatheringsessioncreator7 = new ChurchGatheringManager(repo);
                gatheringsessioncreator7.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("June 12, 2013 5:30 PM"));

                var gatheringsessioncreator8 = new ChurchGatheringManager(repo);
                gatheringsessioncreator8.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("June 15, 2013 8:30 AM"));

                var gatheringsessioncreator9 = new ChurchGatheringManager(repo);
                gatheringsessioncreator9.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("June 15, 2013 5:00 PM"));

                var gatheringsessioncreator10 = new ChurchGatheringManager(repo);
                gatheringsessioncreator10.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("June 19, 2013 5:30 PM"));

                var gatheringsessioncreator11 = new ChurchGatheringManager(repo);
                gatheringsessioncreator11.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("June 22, 2013 8:30 AM"));

                var gatheringsessioncreator12 = new ChurchGatheringManager(repo);
                gatheringsessioncreator12.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("June 22, 2013 5:00 PM"));

                var gatheringsessioncreator13 = new ChurchGatheringManager(repo);
                gatheringsessioncreator13.CreateGathering(prayerMeeting.Gatherings, Convert.ToDateTime("June 26, 2013 5:30 AM"));

                var gatheringsessioncreator14 = new ChurchGatheringManager(repo);
                gatheringsessioncreator14.CreateGathering(worshipService.Gatherings, Convert.ToDateTime("June 29, 2013 8:30 AM"));

                var gatheringsessioncreator15 = new ChurchGatheringManager(repo);
                gatheringsessioncreator15.CreateGathering(thanksGiving.Gatherings, Convert.ToDateTime("June 29, 2013 5:30 PM"));
            }
        }

        [Test]
        public void test_RetrieveGatheringSessionById()
        {
            UnityBootstrapper.Configure();
            var retrieve = UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>();
            var gatheringsessions = retrieve.RetrieveLatestDifferentGatherings();

            var gatheringsession = retrieve.GetGatheringById(gatheringsessions[1].Id);
        }

        [Test]
        public void test_UpdateGathering()
        {
            UnityBootstrapper.Configure();
            var creator = UnityBootstrapper.Container.Resolve<IChurchGatheringManager>();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            var gathering = retriever.GetGatheringById(39);
            gathering.IsStarted = true;
            creator.UpdateGathering(gathering);
        }

        [Test]
        public void test_RemoveGathering()
        {
            UnityBootstrapper.Configure();
            var creator = UnityBootstrapper.Container.Resolve<IChurchGatheringManager>();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            var gathering = retriever.GetGatheringById(39);
            gathering.IsStarted = true;
            creator.RemoveGathering(gathering);
        }

        [Test]
        public void test_IsRemovingSuccessful()
        {
            UnityBootstrapper.Configure();
            var creator = UnityBootstrapper.Container.Resolve<IChurchGatheringManager>();
            var retriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();

            var gathering = retriever.GetGatheringById(35);
            gathering.Id = 36;
            gathering.IsStarted = true;
            creator.RemoveGathering(gathering);

            var IsRemovingSuccesful = creator.IsRemovingSuccessful();

            Assert.IsTrue(IsRemovingSuccesful);
        }
    }
}
