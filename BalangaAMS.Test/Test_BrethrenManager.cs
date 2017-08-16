using NUnit.Framework;
using Microsoft.Practices.Unity;
using BalangaAMS.ApplicationLayer.Service;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class Test_BrethrenManager
    {
        [Test]
        public void Test_addingbrethren()
        {
            UnityBootstrapper.Configure();
            var brethrenmanager = UnityBootstrapper.Container.Resolve<BrethrenManager>();
            var brethren = brethrenmanager.GetBrethrenbyId(49);
            brethrenmanager.Updatebrethren(brethren);
        }

        [Test]
        public void test_updatebrethren()
        {
            UnityBootstrapper.Configure();
            var brethrenManager1 = UnityBootstrapper.Container.Resolve<BrethrenManager>();
            var brethrenManager2 = UnityBootstrapper.Container.Resolve<BrethrenManager>();

            var brethren = brethrenManager1.GetBrethrenbyId(49);
            brethren.ChurchId = "00610865";
            brethrenManager2.Updatebrethren(brethren);
        }

        [Test]
        public void test_deletebrethren()
        {
            UnityBootstrapper.Configure();
            var brethrenManager1 = UnityBootstrapper.Container.Resolve<BrethrenManager>();
            var brethrenManager2 = UnityBootstrapper.Container.Resolve<BrethrenManager>();

            var brethren = brethrenManager1.GetBrethrenbyId(235);
            brethrenManager2.DeleteBrethren(brethren);
        }

        [Test]
        public void test_deletebrethren_in_the_same_Manager()
        {
            UnityBootstrapper.Configure();
            var brethrenManager1 = UnityBootstrapper.Container.Resolve<BrethrenManager>();

            var brethren = brethrenManager1.GetBrethrenbyId(133);
            brethrenManager1.DeleteBrethren(brethren);
        }
    }
}
