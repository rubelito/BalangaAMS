using System;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.ApplicationLayer.Service.GatheringSessionManager;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.Core.Repository;
using Microsoft.Practices.Unity;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using BalangaAMS.ApplicationLayer.Service;
using System.Configuration;
using BalangaAMS.ApplicationLayer.Service.LoggingAttendance;

namespace BalangaAMS.WPF
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

            InitiateRepositoryClass();

            Container.RegisterType<IBrethrenManager, BrethrenManager>();
            Container.RegisterType<IImageService, ImageService>(
                new InjectionConstructor(ConfigurationManager.AppSettings["photodirectory"],
                    ConfigurationManager.AppSettings["defaultphoto"]));
            Container.RegisterType<ILogAuthenticaterByChurchId, LogAuthenticaterByChurchId>();
            Container.RegisterType<IAttendanceLogger, BrethrenAttendancelogger>();
            Container.RegisterType<IChurchGatheringManager, ChurchGatheringManager>();
            Container.RegisterType<IChurchGatheringRetriever, ChurchGatheringRetriever>();
            Container.RegisterType<IAttendanceLogRetriever, AttendanceLogRetriever>();
            Container.RegisterType<IStatusIdentifier, StatusIdentifier>();
            Container.RegisterType<ILateIdentifier, LateIdentifier>();

            Container.RegisterType<IMonthlyReport, MonthlyReportGenerator>();
            Container.RegisterType<IWeeklyReport, WeeklyReportGenerator>();
            Container.RegisterType<IGroupManager, GroupManager>();
            Container.RegisterType<IMonthlyReportSummaryGetter, MonthlyReportSummaryGetter>(
                new InjectionProperty("DaysToConsiderNewlyBaptised",
                    Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"])));
            Container.RegisterType<IExportBrethren, BrethrenExcelExporter>();
            Container.RegisterType<ISettingsManager, SettingsManager>();
            Container.RegisterType<IExportMonthlyAttendanceReport, MonthlyAtttendanceReportExporter>();
            Container.RegisterType<IExportWeeklyAttendanceReport, WeeklyAttendanceReportExporter>();
            Container.RegisterType<IExportDailyAttendanceInfo, DailyAttendanceInfoExporter>();
            Container.RegisterType<IAttendanceRetriever, AttendanceRetriever>(
                new InjectionProperty("DaysToConsiderNewlyBaptised",
                    Convert.ToInt32(ConfigurationManager.AppSettings["daysToConsiderNewlyBaptised"])));
            Container.RegisterType<IAttendeesRetriever, AttendeesRetriever>();
            Container.RegisterType<IChurchIdManager, ChurchIdManager>();
            Container.RegisterType<IOtherLocalManager, OtherLocalLogManager>();
        }

        private static void InitiateRepositoryClass(){
            Container.RegisterType<IBrethrenRepository, BrethrenRepository>();
            Container.RegisterType<IChurchGatheringRepository, ChurchGatheringRepository>();
            Container.RegisterType<IAttendanceLogRepository, AttendanceLogRepository>();
            Container.RegisterType<IGroupRepository, GroupRepository>();
            Container.RegisterType<ICommitteeRepository, CommitteeRepository>();
            Container.RegisterType<IGatheringScheduleRepository, GatheringScheduleRepository>();
            Container.RegisterType<IChurchIdRepository, ChurchIdRepository>();
            Container.RegisterType<IOtherLocalLogRepository, OtherLocalLogRepository>();

        }
    }
}
