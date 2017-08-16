using System.Collections.Generic;
using System.Configuration;
using System.Net.Mime;
using BalangaAMS.Core.Domain;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using BalangaAMS.Core.HelperDomain;
using NUnit.Framework;
using System;
using System.Linq;
using Microsoft.Practices.Unity;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_something
    {
        [Test]
        public void test_i_dont_know()
        {
            var connection = new EfSQLite("SQLiteDb");
            using (var repo = new AMSUnitofWork(connection))
            {
                var brethren = repo.BrethrenBasics.Find(b => b.Id == 4).FirstOrDefault();
                var committee = repo.Committees.Find(c => c.BrethrenBasics.All(b => b.Id != brethren.Id)).ToList();
            }
        }

        [Test]
        public void test_searchstr()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                var brethrens = repo.BrethrenBasics.FindAll().ToList();

                var brethren =
                    brethrens.Where(b => b.Name.IndexOf("RUBELITO", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        .ToList();
            }
        }

        [Test]
        public void test_replace_group()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                var brethren = repo.BrethrenBasics.Find(b => b.Group.Id == 1).FirstOrDefault();

                var group = repo.Groups.Find(g => g.Id == 2).FirstOrDefault();

                brethren.Group = group;

                repo.Commit();
            }
        }

        [Test]
        public void test_querymonth()
        {
            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                int month = Convert.ToInt32(MonthofYear.December);
                var brethrens =
                    repo.BrethrenBasics.Find(
                        b => b.BrethrenFull.DateofBaptism.Value.Month == month).ToList();

            }
        }

        [Test]
        public void test_date()
        {
            DateTime timenow = Convert.ToDateTime("Jun 4, 2013");
        }

        [Test]
        public void test_extractDataStr()
        {
            string sampleStr = removedtcol("dtcol_W_03S21S14");
            string sampleStr1 = removedtcol("dtcol_P_Total");


        }

        private string removedtcol(string str)
        {
            string finalstr;
            if (str.Contains("Total"))
            {
                finalstr = str.Substring(8, str.Length - 8);
            }
            else
            {
                str = str.Substring(8, str.Length - 8);
                string[] splitDate = str.Split('S');
                finalstr = splitDate[0] + "/" + splitDate[1] + "/" + splitDate[2];
            }
            return finalstr;
        }

        [Test]
        public void find_longestBrethrenName()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethren = brethrenManager.GetAllBrethren().OrderByDescending(b => b.Name.Length).FirstOrDefault();
        }

        [Test]
        public void test_subtractMonth()
        {
            var dt = DateTime.Now.AddMonths(1);
        }

        [Test]
        public void test_GatheringSessionRetriever()
        {
            UnityBootstrapper.Configure();
            var sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var gatherings = sessionRetriever.GetAllGatheringsForMonthOf(MonthofYear.July, 2013);
        }

        [Test]
        public void test_Check_if_brethren_is_newly_baptised()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethrenList = brethrenManager.FindBrethren(b => b.BrethrenFull.DateofBaptism.HasValue);
            var newlyBaptised = new List<BrethrenBasic>();
            var mostRecent = brethrenList.OrderByDescending(b => b.BrethrenFull.DateofBaptism.Value).FirstOrDefault();
            var daysToConsiderNewlyBaptised = Convert.ToDouble(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"]);

            foreach (var brethren in brethrenList)
            {
                if ((DateTime.Now - brethren.BrethrenFull.DateofBaptism.Value).TotalDays <= daysToConsiderNewlyBaptised)
                {
                    newlyBaptised.Add(brethren);
                }                   
            }
        }

        //[Test]
        //public void test_get_different_year_and_month()
        //{
        //    UnityBootstrapper.Configure();
        //    var sessionRetriever = UnityBootstrapper.Container.Resolve<IGatheringSessionRetriever>();

        //    var sessions = sessionRetriever.FindGatheringSessions(g => g.IsStarted);
        //    var years = sessions.DistinctBy(g => g.Date.Year).Select(g => g.Date.Year).ToList();

        //    var monthYears = new List<MonthYear>();

        //    foreach (var session in sessions)
        //    {
        //        if (!IsMonthYearAdded(monthYears, session.Date.Year, session.Date.Month))
        //        {
        //            monthYears.Add(new MonthYear(){Year = session.Date.Year, Month = session.Date.Month});
        //        }
        //    }
        //}

        private bool IsMonthYearAdded(List<MonthYear> monthYear, int year, int month)
        {
            return monthYear.Any(g => g.Year == year && g.Month == month);
        }

        public class MonthYear
        {
            public int Year { get; set; }
            public int Month { get; set; }
        }

        [Test]
        public void test_Math_Division()
        {
            var result = 14/2;
            Console.WriteLine(result);
        }
    }
}
