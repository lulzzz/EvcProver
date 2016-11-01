using System.Data.Entity.Migrations;
using Prover.Core.Storage;

namespace Prover.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ProverContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Prover.Core.Storage.ProverContext";
        }

        protected override void Seed(ProverContext context)
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