using System;
using System.Reflection;
using Moq;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Tracky.Domain.EF.Music;
using Tracky.Orm.NH.GabrielSchenker;

namespace Tracky.Tests.Unit.Orm.NH.GabrielSchenker
{
    [TestFixture]
    class UnitOfWorkManagerTests
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Default);
        private Mock<IUnitOfWorkFactory> _mockFactory;
        private Mock<IUnitOfWorkImplementor> _mockUow;
        private Mock<ISession> _mockSession;

        [SetUp]
        public void SetUpContext()
        {
            _mockFactory = new Mock<IUnitOfWorkFactory>();
            _mockUow = new Mock<IUnitOfWorkImplementor>();
            _mockSession = new Mock<ISession>();
            _mockFactory.Setup(f => f.Create()).Returns(_mockUow.Object);
            _mockFactory.Setup(f => f.CurrentSession).Returns(_mockSession.Object);

            // Using reflection here to set the value of a private field of the UnitOfWorkManager
            // class with our mocked factory, via brute force. A public setter on the UnitOfWorkManager
            // class would also allow for this, but we shouldn't need it beyond these tests.
            var fieldInfo = typeof(UnitOfWorkManager).GetField("_unitOfWorkFactory",
                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(null, _mockFactory.Object);
        }

        [Test]
        public void CanStartUnitOfWork()
        {
            IUnitOfWorkImplementor uow = UnitOfWorkManager.Start();

            Assert.IsNotNull(uow);
            _mockFactory.Verify(f => f.Create(), Times.Once);
        }

        [Test]
        public void ShouldThrowErrorIfStartingUowWhenAlreadyStarted()
        {
            UnitOfWorkManager.Start();
            var isException = false;

            try
            {
                UnitOfWorkManager.Start();
            }
            catch(InvalidOperationException ex)
            {
                isException = true;
            }

            Assert.That(isException, Is.True);
            _mockFactory.Verify(f => f.Create(), Times.Once);
        }

        [Test]
        public void CanAccessCurrentUnitOfWork()
        {
            IUnitOfWorkImplementor uow = UnitOfWorkManager.Start();
            var current = UnitOfWorkManager.Current;

            Assert.That(uow, Is.SameAs(current));
            _mockFactory.Verify(f => f.Create(), Times.Once);
        }

        [Test]
        public void ShouldThrowErrorIfAccessingCurrentUowWhenNoUowHasBeenStarted()
        {
            var isException = false;

            try
            {
                var current = UnitOfWorkManager.Current;
            }
            catch (InvalidOperationException ex)
            {
                isException = true;
            }

            Assert.That(isException, Is.True);
        }

        [Test]
        public void CanTestIfUowIsStarted()
        {
            Assert.That(UnitOfWorkManager.IsStarted, Is.False);

            IUnitOfWorkImplementor uow = UnitOfWorkManager.Start();
            
            Assert.That(UnitOfWorkManager.IsStarted, Is.True);
        }

        [Test]
        public void CanGetValidCurrentSessionIfUowIsStarted()
        {
            using (UnitOfWorkManager.Start())
            {
                ISession session = UnitOfWorkManager.CurrentSession;
                Assert.That(session, Is.Not.Null);
                _mockFactory.VerifyAll();
            }
        }

        [TearDown]
        public void TearDownContext()
        {
            // Use reflection again to set inner uow back to null after each test
            var fieldInfo = typeof(UnitOfWorkManager).GetField("_unitOfWorkImplementor",
                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(null, null);
        }

    }

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        //[Ignore("This test represents the failing point of trying to implement Gabriel Schenker's Unit of Work pattern" +
        //        "from http://nhibernate.info/doc/patternsandpractices/nhibernate-and-the-unit-of-work-pattern.html. This" +
        //        "test is mentioned at the end of Part 2 in the Using the Unit of Work section, but does not seem to work" +
        //        "the way he intends. Perhaps this is due to the almost 10-year-old nature of the article, and differences" +
        //        "between NHibernate then and now, but it is the only Unit of Work implementation that is documented on the" +
        //        "official NHibernate.info website.")]
        public void ShouldUseUnitOfWorkToActuallyWriteToDatabase()
        {
            UnitOfWorkManager.Factory = new UnitOfWorkFactory();
            //UnitOfWorkManager.Configuration.AddAssembly(Assembly.GetExecutingAssembly()); // Used if mapping is local to test project (which it's currently not)
            UnitOfWorkManager.Configuration.AddAssembly("Tracky"); // Used to point to the primary project to find the NHibernate mapping

            // This step generates a "There is already an object named 'Authors' in the database." error. WTF?
            // Without it, the test continues and the Current Session seems to accept the Save(artist) call, but
            // an ambiguous stack overflow error is encountered upon calling TransactionalFlush.
            new SchemaExport(UnitOfWorkManager.Configuration).Execute(true, true, true);

            using (UnitOfWorkManager.Start())
            {
                var artist = new Artist { Name = "Rock Musician " + new Random().Next(0, 90000000) };
                UnitOfWorkManager.CurrentSession.Save(artist);
                UnitOfWorkManager.Current.TransactionalFlush(); // This flush causes a stack overflow error in the Dispose method of UnitOfWorkImplementor. WTF?
            }
        }
    }
}
