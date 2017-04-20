using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NHibernate;
using NUnit.Framework;
using Tracky.Domain.Repositories;

namespace Tracky.Tests.Unit.Domain.Repositories
{
    [TestFixture]
    class UnitOfWorkFactoryTests
    {
        private IUnitOfWorkFactory _unitOfWorkFactory;

        [SetUp]
        public void SetUpContext()
        {
            // The default constructor of UnitOfWorkFactory class is internal, which keeps the construction
            // of a new factory instance internal to the assembly implementing the Unit of Work pattern.
            // Thus, the Activator technique is used from this test assembly to create an instance.
            _unitOfWorkFactory = (IUnitOfWorkFactory)Activator.CreateInstance(typeof(UnitOfWorkFactory), true);
        }

        [Test]
        [Ignore("until the issue of finding the right database has been corrected")]
        public void CanCreateUnitOfWork()
        {
            IUnitOfWork implementor = _unitOfWorkFactory.Create();

            Assert.That(implementor, Is.Not.Null);
            Assert.That(_unitOfWorkFactory.CurrentSession, Is.Not.Null);
            Assert.That(_unitOfWorkFactory.CurrentSession.FlushMode, Is.EqualTo(FlushMode.Commit));
        }

        // THIS TEST WORKS! It reads the hibernate.cfg.xml file in from the copy to the Debug folder at
        // run time and verifies all of the properties. The only problem, demonstrated in the other tests,
        // seems to be that it's not finding the database correctly. It's looking to find it here in the
        // test project, but no no no it's not there, yo. It lives in the primary web project where the
        // EF implementation is that created it, at Tracky/App_Data. But even when these tests are copied
        // over to that main project, and the path to the DB appears to be correct, an error still tells
        // us that it cannot be openend. So...how to point NHibernate to a DB created via EF? That is
        // still a mystery that needs to be solved.
        // ----
        // What's interesting is that the code in SillyStarterStuff.cs works fine, in the main project,
        // interacting with the database correctly. Study the setup that is allowing that and think about
        // what might need to be done for this code to work with the database correctly.
        // ----
        // Upon further testing, it seems the problem is that the localdb type database that we are using
        // here, built by EF, is not accessible to NHibernate when running unit tests, though the same
        // code does work fine when the app is running.
        [Test]
        public void CanConfigureNHibernate()
        {
            var configuration = _unitOfWorkFactory.Configuration;
            
            Assert.That(configuration, Is.Not.Null);
            Assert.AreEqual("NHibernate.Connection.DriverConnectionProvider",
                    configuration.Properties["connection.provider"]);
            Assert.AreEqual("NHibernate.Dialect.MsSql2012Dialect",
                            configuration.Properties["dialect"]);
            Assert.AreEqual("NHibernate.Driver.SqlClientDriver",
                            configuration.Properties["connection.driver_class"]);
            Assert.AreEqual("Data Source=(localdb)\\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True; AttachDbFilename=|DataDirectory|LibraryContext-20161220125911.mdf",
                            configuration.Properties["connection.connection_string"]);
        }

        [Test]
        [Ignore("until the issue of finding the right database has been corrected")]
        public void CanCreateAndAccessSessionFactory()
        {
            var sessionFactory = _unitOfWorkFactory.SessionFactory;
            Assert.That(sessionFactory, Is.Not.Null);
            //Assert.That(sessionFactory.Dialect.ToString(), Is.EqualTo("NHibernate.Dialect.MsSql2012Dialect")); // TODO: How to check Dialect on session factory?
        }

    }
}
