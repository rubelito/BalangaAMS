using System;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using Microsoft.Practices.Unity;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using BalangaAMS.ApplicationLayer.Service;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using System.Configuration;
using BalangaAMS.ApplicationLayer.Service.LoggingAttendance;

namespace BalangaAMS.Test
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
            Container.RegisterType<IUnitofWork, AMSUnitofWork>();
            Container.RegisterType<IBrethrenManager, BrethrenManager>();
            Container.RegisterType<IChurchGatheringManager, ChurchGatheringManager>();
            Container.RegisterType<IChurchGatheringRetriever, ChurchGatheringRetriever>();
            Container.RegisterType<IAttendanceLogRetriever, AttendanceLogRetriever>();
            Container.RegisterType<IStatusIdentifier, StatusIdentifier>();
            Container.RegisterType<ILateIdentifier, LateIdentifier>(new InjectionProperty("NumberOfLate",
                                                                                          Convert.ToInt32(
                                                                                          ConfigurationManager
                                                                                              .AppSettings[
                                                                                                  "NumberOfLate"])));
            Container.RegisterType<IGroupManager, GroupManager>();
            Container.RegisterType<IAttendanceLogger, BrethrenAttendancelogger>();
            Container.RegisterType<IExportBrethren, BrethrenExcelExporter>();
        }
    }
}
