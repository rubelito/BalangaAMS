using System.Linq;
using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_LateIdentifier
    {
        [Test]
        public void test_CountBrethrenLateForMonthOf()
        {
            UnityBootstrapper.Configure();

            var brethren =
               UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                .FindBrethren(b => b.ChurchId == "00610865")
                                .FirstOrDefault();

            var lateIdentifier = UnityBootstrapper.Container.Resolve<ILateIdentifier>();
            var latecount = lateIdentifier.CountTheLateOfBrethrenForMonthOf(brethren.Id, MonthofYear.May, 2013);           
        }

        [Test]
        public void test_GetlatebrethrensInSession()
        {
            UnityBootstrapper.Configure();
            var sessionRetriever = UnityBootstrapper.Container.Resolve<IChurchGatheringRetriever>();
            var gatheringSession = sessionRetriever.GetGatheringById(8);
            var lateIdentifier = UnityBootstrapper.Container.Resolve<ILateIdentifier>();
            var latebrethren = lateIdentifier.GetLateBrethrensInSession(gatheringSession.Id);
        }

        [Test]
        public void test_IsBrethrenIsLate()
        {
            //"00902206"
            //"00610865"
            //"00905363"
            //"B1500002"
            //"B1500003"
            UnityBootstrapper.Configure();

            var brethren =
               UnityBootstrapper.Container.Resolve<BrethrenManager>()
                                .FindBrethren(b => b.ChurchId == "B1500002")
                                .FirstOrDefault();

            var lateIdentifier = UnityBootstrapper.Container.Resolve<ILateIdentifier>();
            var islate = lateIdentifier.IsBrethrenIsLate(brethren.Id, 11);
            Assert.IsTrue(islate);
        }
    }
}
