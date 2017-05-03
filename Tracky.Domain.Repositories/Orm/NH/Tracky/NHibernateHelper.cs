using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.Orm.NH.Tracky
{
    public sealed class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private const string ProductionConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=C:\dev\dotnet\TrackyDotNet\Tracky\App_Data\LibraryContext-20161220125911.mdf;Integrated Security=True;MultipleActiveResultSets=True";
        private const string TestConnectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\dev\dotnet\TrackyDotNet\Tracky.Tests.Integration\App_Data\TrackyTesting.mdf;Integrated Security=True;Connect Timeout=30";

        /// <summary>
        /// GetSessionFactory checks the private static property to see if a
        /// session factory exists yet, and if so, returns it, otherwise it
        /// will create it. This guarantees our session factory is a singleton.
        /// 
        /// The function takes a boolean telling it whether we are using it
        /// for a test or not, in which case, a separate database is used,
        /// which we instantiate with a fresh schema for each test run.
        /// </summary>
        /// <returns>ISessionFactory</returns>
        public static ISessionFactory GetSessionFactory(bool forTesting)
        {
            var connectionString = forTesting ? TestConnectionString : ProductionConnectionString;

            if (_sessionFactory == null)
            {
                var configuration = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012
                        .ConnectionString(connectionString)
                        .ShowSql())
                    .Mappings(m => m.FluentMappings
                        .AddFromAssemblyOf<Artist>())
                    .BuildConfiguration();

                if (forTesting)
                {
                    var schemaExporter = new SchemaExport(configuration);
                    schemaExporter.Execute(true, true, false);
                }

                _sessionFactory = configuration.BuildSessionFactory();
            }
            return _sessionFactory;
        }

        public static ISession OpenSession(bool forTesting)
        {
            return GetSessionFactory(forTesting).OpenSession();
        }
    }
}