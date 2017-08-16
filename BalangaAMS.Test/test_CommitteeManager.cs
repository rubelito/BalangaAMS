using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.Core.Domain;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using NUnit.Framework;
using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_CommitteeManager
    {

        [Test]
        public void AdddingCommitteeShoulBeSuccessfulIfThisCommitteeIsNotExistedYet()
        {
            UnityBootstrapper.Configure();
            var committeemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();

            committeemanager.AddCommittee(new Committee()
                {
                    Name = "Theatro Kristiano"
                });

            Console.WriteLine(committeemanager.Statusmessage());
        }

        [Test]
        public void AddingCommitteeThatAlreadyExistedShoulFail()
        {
            UnityBootstrapper.Configure();
            var committeemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();

            committeemanager.AddCommittee(new Committee()
            {
                Name = "QUAT"
            });

            Console.WriteLine(committeemanager.Statusmessage());
        }

        [Test]
        public void RemovingCommitteeThatExistedWillBeSuccessful()
        {
            UnityBootstrapper.Configure();
            var committeemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();

            committeemanager.RemoveCommittee(new Committee()
            {
                Id = 6,
                Name = "QUAT"
            });

            Console.WriteLine(committeemanager.Statusmessage());
        }
        [Test]
        public void RemovingCommitteeThatDidNotExistedYetWillResultToFailure()
        {
            UnityBootstrapper.Configure();
            var committeemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();
            
            committeemanager.RemoveCommittee(new Committee()
                {
                    Id = 9,
                    Name = "sgsfd"
                });

            Console.WriteLine(committeemanager.Statusmessage());
        }

        [Test]
        public void add_committee()
        {

            UnityBootstrapper.Configure();
            using (var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>())
            {
                var committee1 = new Committee()
                    {
                        Name = "GCos",
                    };
                var committee2 = new Committee()
                {
                    Name = "KKTK",
                };
                var committee3 = new Committee()
                {
                    Name = "Theatro Kristiano",
                };
                var committee4 = new Committee()
                {
                    Name = "Choir",
                };
                var committee5 = new Committee()
                {
                    Name = "Mothers Club",
                };
                var committee6 = new Committee()
                {
                    Name = "QUAT",
                };
                var committee7 = new Committee()
                {
                    Name = "Worker",
                };

                repo.Committees.Add(committee1);
                repo.Committees.Add(committee2);
                repo.Committees.Add(committee3);
                repo.Committees.Add(committee4);
                repo.Committees.Add(committee5);
                repo.Committees.Add(committee6);
                repo.Committees.Add(committee7);

                repo.Commit();
            }     
        }

        [Test]
        public void add_committee_to_brethren()
        {
            UnityBootstrapper.Configure();
            using (var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>())
            {
                var brethren = repo.BrethrenBasics.Find(b => b.ChurchId == "11009773").FirstOrDefault();
                var committeelist = repo.Committees.FindAll().OrderBy(c => c.Id).Skip(2).Take(4).ToList();

                foreach (var committee in committeelist)
                {
                    brethren.Committees.Add(committee);
                }

                repo.Commit();
            }
        }

        [Test]
        public void update_committee()
        {
            UnityBootstrapper.Configure();
            var commiteemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();

            var committee = new Committee();
            committee.Id = 5;
            committee.Name = "Mothers Club";
           
            commiteemanager.UpdateCommitee(committee);
        }

        [Test]
        public void Remove_brethrenFromCommittee()
        {
            UnityBootstrapper.Configure();
            var commiteemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();
            commiteemanager.RemoveBrethrenFromCommittee(1,6);
        }

        [Test]
        public void Test_GetBrethrenInThisCommittee()
        {
            UnityBootstrapper.Configure();
            var repo = UnityBootstrapper.Container.Resolve<AMSUnitofWork>();
            var brethren = repo.BrethrenBasics.Find(b => b.Committees.Any(c => c.Id == 2)).ToList();
        }

        [Test]
        public void Test_AddbrethrentoCommittee()
        {
            UnityBootstrapper.Configure();
            var commiteemanager = UnityBootstrapper.Container.Resolve<CommitteeManager>();
            commiteemanager.AddBrethrenToCommittee(239,5);
        }
    }
}
