using BalangaAMS.ApplicationLayer;
using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BalangaAMS.Core.Interfaces;
using System.Drawing;
using System.Configuration;

namespace BalangaAMS.Test
{
    [TestFixture]
    class Test_ReportManager
    {
        [Test]
        public void do_something()
        {

            var checkImg = Image.FromFile(ConfigurationManager.AppSettings["checkImg"]);
            var crossImg = Image.FromFile(ConfigurationManager.AppSettings["crossImg"]);

            UnityBootstrapper.Configure();
            var statusIdentifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var brethrenMangaer = UnityBootstrapper.Container.Resolve<IBrethrenManager>();

            var gatheringSession = sessionRetriever.GetAllGatheringsForMonthOf(MonthofYear.May, 2013);

            var prayerMeeting =
                gatheringSession.Where(
                    g => g.Gatherings == Gatherings.Prayer_Meeting)
                                .ToList();

            var worshipSerive = gatheringSession.Where(g => g.Gatherings == Gatherings.Worship_Service).ToList();
            var thanksGiving = gatheringSession.Where(g => g.Gatherings == Gatherings.Thanks_Giving).ToList();

            var brethren = brethrenMangaer.FindBrethren(b => b.ChurchId == "00610865").FirstOrDefault();

            var sessionAttended = sessionRetriever.GetGatheringsThatBrethrenAttendedForTheMonthOf(brethren.Id,
                                                                                                        MonthofYear.May,
                                                                                                        2013);

            Dictionary<string, object> brethrenReport = new Dictionary<string, object>();
            
            brethrenReport.Add("ChurchId", brethren.ChurchId);
            brethrenReport.Add("Name", brethren.Name);

            foreach (GatheringSession session in prayerMeeting)
            {
                Image statusImg = sessionAttended.Contains(session) ? checkImg : crossImg;
                brethrenReport.Add("P " + session.Date.ToShortDateString(), statusImg);
            }

            foreach (GatheringSession session in worshipSerive)
            {
                Image statusImg = sessionAttended.Contains(session) ? checkImg : crossImg;
                brethrenReport.Add("W " + session.Date.ToShortDateString(), statusImg);
            }

            foreach (GatheringSession session in thanksGiving)
            {
                Image statusImg = sessionAttended.Contains(session) ? checkImg : crossImg;
                brethrenReport.Add("T " + session.Date.ToShortDateString(), statusImg);
            }
        }

        [Test]
        public void test_generatereport()
        {
            UnityBootstrapper.Configure();

            var monthlyReport = UnityBootstrapper.Container.Resolve<MonthlyReportGenerator>();

            var reportTable = monthlyReport.GenerateBrethrenReport(49, MonthofYear.May, 2013);

        }
    }
}
