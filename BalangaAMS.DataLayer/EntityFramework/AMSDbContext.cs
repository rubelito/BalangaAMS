using System.Data.Entity;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.DataLayer.EntityFramework
{
    internal class AMSDbContext: DbContext
    {
        public AMSDbContext(){


        }

        public AMSDbContext(IDatabaseType databaseType):
            base(databaseType.Connectionstring(),true)
        {
           
        }

        public DbSet<BrethrenBasic> BrethrenBasics { get; set; }
        public DbSet<BrethrenFull> BrethrenFulls { get; set; }
        public DbSet<BrethrenFingerPrint> BrethrenFingerPrints { get; set; }
        public DbSet<Group> Groups { get; set; } 
        public DbSet<Committee> Committees { get; set; }
        public DbSet<GatheringSession> GatheringSessions { get; set; }
        public DbSet<GatheringSchedule> GatheringSchedules { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<OtherLocalLog> OtherLocalLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrethrenBasic>().HasRequired(b => b.BrethrenFull)
                .WithRequiredPrincipal();

            modelBuilder.Entity<BrethrenBasic>().ToTable("Brethren");
            modelBuilder.Entity<BrethrenFull>().ToTable("Brethren");
        }
    }
}
