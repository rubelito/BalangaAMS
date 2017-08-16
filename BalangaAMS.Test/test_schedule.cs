using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.ApplicationLayer.Service.LoggingAttendance;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.DataLayer;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using BalangaAMS.ApplicationLayer.Service;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_schedule
    {
        [Test]
        public void test_adding_schedules()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                var schedule2 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Wednesday,
                    Gatherings = Gatherings.Prayer_Meeting,
                    Time = "5:30 PM",
                    MinutesBeforePrayer = 30
                };
                var schedule3 = new GatheringSchedule()
                {
                    Day =  DayOfWeek.Thursday,
                    Gatherings = Gatherings.Prayer_Meeting,
                    Time = "8:30 AM",
                    MinutesBeforePrayer = 30
                };
                var schedule4 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Thursday,
                    Gatherings = Gatherings.Prayer_Meeting,
                    Time = "5:30 PM",
                    MinutesBeforePrayer = 30
                };
                var schedule5 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Saturday,
                    Gatherings = Gatherings.Worship_Service,
                    Time = "8:30 AM",
                    MinutesBeforePrayer = 30
                };
                var schedule6 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Saturday,
                    Gatherings = Gatherings.Worship_Service,
                    Time = "2:30 PM",
                    MinutesBeforePrayer = 30
                };
                var schedule7 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Saturday,
                    Gatherings = Gatherings.Thanks_Giving,
                    Time = "5:30 PM",
                    MinutesBeforePrayer = 30
                };
                var schedule8 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Sunday,
                    Gatherings = Gatherings.Worship_Service,
                    Time = "8:30 AM",
                    MinutesBeforePrayer = 30
                };
                var schedule9 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Sunday,
                    Gatherings = Gatherings.Thanks_Giving,
                    Time = "11:00 AM",
                    MinutesBeforePrayer = 30
                };
                var schedule10 = new GatheringSchedule()
                {
                    Day = DayOfWeek.Sunday,
                    Gatherings = Gatherings.Worship_Service,
                    Time = "5:30 PM",
                    MinutesBeforePrayer = 30
                };
                
                repo.GatheringSchedules.Add(schedule2);
                repo.GatheringSchedules.Add(schedule3);
                repo.GatheringSchedules.Add(schedule4);
                repo.GatheringSchedules.Add(schedule5);
                repo.GatheringSchedules.Add(schedule6);
                repo.GatheringSchedules.Add(schedule7);
                repo.GatheringSchedules.Add(schedule8);
                repo.GatheringSchedules.Add(schedule9);
                repo.GatheringSchedules.Add(schedule10);
               
                repo.Commit();
            }
        }

        [Test]
        public void test_getlistofschedule()
        {
            UnityBootstrapper.Configure();
            var schedulemanager = UnityBootstrapper.Container.Resolve<ScheduleManager>();
            var schedules = schedulemanager.GetListofSchedule();
        }

        [Test]
        public void test_addschedule()
        {
            UnityBootstrapper.Configure();
            var schedulemanager = UnityBootstrapper.Container.Resolve<ScheduleManager>();
            var gatheringschedule = new GatheringSchedule();
            gatheringschedule.Day = DayOfWeek.Thursday;
            gatheringschedule.Time = "6:30 PM";
            gatheringschedule.MinutesBeforePrayer = 30;
            gatheringschedule.Gatherings = Gatherings.Prayer_Meeting;
            schedulemanager.AddSchedule(gatheringschedule);
        }
    }
}
