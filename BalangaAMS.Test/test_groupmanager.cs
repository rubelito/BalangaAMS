using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.Core.Domain;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using System.Linq;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_groupmanager
    {
        [Test]
        public void test_addinggroup()
        {
            UnityBootstrapper.Configure();
            using (var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>())
            {
                for (int i = 1; i < 14; i++)
                {
                    var group = new Group()
                        {
                            GroupName = i.ToString()
                        };
                    var groupmanager = new GroupManager(repo);
                    groupmanager.Addgroup(group);
                }
            }
        }

        [Test]
        public void test_removinggroup()
        {
            UnityBootstrapper.Configure();
            using (var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>())
            {
                var group = repo.Groups.Find(g => g.Id == 10).FirstOrDefault();

                var groupmanager = new GroupManager(repo);
                groupmanager.Removegroup(group);

            }
        }

        [Test]
        public void add_nogroup()
        {
            var group = new Group();
            group.GroupName = "No group";
            var groupmanager = UnityBootstrapper.Container.Resolve<GroupManager>();
            groupmanager.Addgroup(group);    
        }
      
        [Test]
        public void add_brehtrenToAGroup()
        {
            UnityBootstrapper.Configure();
            var groupmanager = UnityBootstrapper.Container.Resolve<GroupManager>();
            groupmanager.AddBrethrenToAGroup(49,22);
        }

        [Test]
        public void add_group()
        {
            var group = new Group();
            group.GroupName = "15";
            UnityBootstrapper.Configure();
            var groupmanager = UnityBootstrapper.Container.Resolve<GroupManager>();
            groupmanager.Addgroup(group);  
        }

        [Test]
        public void removeBrethrenToAGroup()
        {
            UnityBootstrapper.Configure();
            var groupmanager = UnityBootstrapper.Container.Resolve<GroupManager>();
            groupmanager.RemoveBrethrenToAGroup(49);
        }
    }
 }

