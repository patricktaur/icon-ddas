namespace DDAS.EF.Migrations
{
    using Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DDAS.EF.ApplicationIdentityDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(DDAS.EF.ApplicationIdentityDBContext context)
        {
            //  This method will be called after migrating to the latest version.


            context.Params = context.Set<Param>();

            context.Params.AddOrUpdate(p => p.RecId,
                new Param { RecId = 100, CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now, Description = "Gender" },
                new Param { RecId = 101, CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now, Description = "Male", ParId = 100 },
                new Param { RecId = 102, CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now, Description = "Female", ParId = 100 }
                );


            context.SaveChanges();


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
