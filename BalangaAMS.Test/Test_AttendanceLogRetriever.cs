using System.Linq;
using BalangaAMS.ApplicationLayer.Service;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;

namespace BalangaAMS.Test
{
    [TestFixture]
    class Test_AttendanceLogRetriever
    {
        [Test]
        public void test_GetBrethrenAttendanceLogForMonthOf()
        {
            UnityBootstrapper.Configure();

            //var LogRetriever = UnityBootstrapper.Container.Resolve<AttendanceLogRetriever>();
            //var attendancelog = LogRetriever.GetBrethrenAttendanceLogForMonthOf("00610865", MonthofYear.June, 2013);
            //var lateattendace = attendancelog.Where(a => a.IsLate == true).ToList();

            var brethren =
                UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                 .FindBrethren(b => b.ChurchId == "00610865")
                                 .FirstOrDefault();

            var gatheringsesion =
                UnityBootstrapper.Container.Resolve<ChurchGatheringRetriever>().GetGatheringById(5);
            var logretriver = UnityBootstrapper.Container.Resolve<AttendanceLogRetriever>();
            var attendancelog = logretriver.GetBrethrenAttendanceLogInSession(brethren.Id, gatheringsesion);
            
        }
    }
}
