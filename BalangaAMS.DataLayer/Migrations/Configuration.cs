using Devart.Data.SQLite.Entity.Migrations;

namespace BalangaAMS.DataLayer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BalangaAMS.DataLayer.EntityFramework.AMSDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            var connectionInfo = SQLiteConnectionInfo.GetConnection("SQLiteDb");
            this.TargetDatabase = connectionInfo;
            this.SetSqlGenerator(connectionInfo.GetInvariantName(),
                   new SQLiteEntityMigrationSqlGenerator());

        }

        protected override void Seed(BalangaAMS.DataLayer.EntityFramework.AMSDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
