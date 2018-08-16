using System.Data.Entity;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;
using Tracky.Migrations;

namespace Tracky.Tests.Integration
{
    /// <summary>
    /// The EfDatabaseInitializer is used for Entity Framework integration tests,
    /// not NHibernate integration tests, to provide a freshly built in-memory
    /// database for each test, seeded with starter data.
    /// </summary>
    public class EfDatabaseInitializer : DropCreateDatabaseAlways<LibraryContext>
    {
        protected override void Seed(LibraryContext context)
        {
            DataSeeder.SeedData(context);

            base.Seed(context);
        }
    }
}
