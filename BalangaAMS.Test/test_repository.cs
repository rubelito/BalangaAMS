using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BalangaAMS.DataLayer.Repository;
using BalangaAMS.Core.Domain;
using BalangaAMS.DataLayer.EntityFramework;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_repository
    {
        [Test]
        public void test_detachobject()
        {
            //UnityBootstrapper.Configure();
            Group group;// = new Group();

            var connection = new EfSQLite("SQLiteDb"); ;

            using (var repo = new AMSUnitofWork(connection))
            {
                group = repo.Groups.Find(g => g.Id == 14).FirstOrDefault();
            }
            group.Id = 14;
            group.GroupName = "14";

            using (var repo = new AMSUnitofWork(connection))
            {
                repo.Groups.Update(group);
                repo.Commit();
            }

        }
    }
}
