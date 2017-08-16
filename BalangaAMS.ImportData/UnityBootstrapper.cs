using BalangaAMS.ApplicationLayer.ImportExcelData;
using BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData;
using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Repository;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using Microsoft.Practices.Unity;

namespace BalangaAMS.ImportData
{
    public class UnityBootstrapper
    {
        public static UnityContainer Container;

        public static void Configure()
        {
            if (Container == null)
            {
                Container = new UnityContainer();
            }

            Container.RegisterType<IDatabaseType, EfSQLite>(new InjectionConstructor("SQLiteDb"));
            Container.RegisterType<IGroupRepository, GroupRepository>();
            Container.RegisterType<IBrethrenRepository, BrethrenRepository>();
            Container.RegisterType<IBrethrenManager, BrethrenManager>();
            Container.RegisterType<IGroupManager, GroupManager>();
            Container.RegisterType<IImporttoDb, ImportbrethrentoDb>();
        }
    }
}
