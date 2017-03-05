using System.Data.Entity;
using Tracky.Domain.EF.DataContexts;
using Tracky.Migrations;

namespace Tracky.Tests.Integration
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<LibraryContext>
    {
        protected override void Seed(LibraryContext context)
        {
            DataSeeder.SeedData(context);

            base.Seed(context);
        }
    }
}
