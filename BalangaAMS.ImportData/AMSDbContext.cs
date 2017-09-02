using System.Data.Entity;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ImportData
{
    internal class AMSDbContext : DbContext
    {
        public AMSDbContext(IDatabaseType databaseType) :
            base(databaseType.Connectionstring(), true)
        {

        }

        public AMSDbContext()
        {
            // TODO: Complete member initialization
        }

        public DbSet<BrethrenBasic> BrethrenBasics { get; set; }
        public DbSet<BrethrenFull> BrethrenFulls { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<GatheringSession> GatheringSessions { get; set; }
        public DbSet<GatheringSchedule> GatheringSchedules { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<OtherLocalLog> OtherLocalLogs { get; set; }
        public DbSet<ChurchId> ChurchIds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrethrenBasic>().ToTable("Brethren");
            modelBuilder.Entity<BrethrenFull>().ToTable("Brethren");
            modelBuilder.Entity<BrethrenBasic>().HasRequired(b => b.BrethrenFull)
                        .WithRequiredPrincipal();
        }
    }
}
