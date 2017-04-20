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
    class UnitOfWorkTests
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Default);
        private Mock<IUnitOfWorkFactory> _mockFactory;
        private Mock<IUnitOfWork> _mockUow;
        private Mock<ISession> _mockSession;

        [SetUp]
        public void SetUpContext()
        {
            _mockFactory = new Mock<IUnitOfWorkFactory>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockSession = new Mock<ISession>();
            _mockFactory.Setup(f => f.Create()).Returns(_mockUow.Object);
            _mockFactory.Setup(f => f.CurrentSession).Returns(_mockSession.Object);

            // Using reflection here to set the value of a private field of the UnitOfWork
            // class with our mocked factory, via brute force. A public setter on the UnitOfWork
            // class would also allow for this, but we shouldn't need it beyond these tests.
            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory",
                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(null, _mockFactory.Object);
        }

        [Test]
        public void CanStartUnitOfWork()
        {
            IUnitOfWork uow = UnitOfWork.Start();

            Assert.IsNotNull(uow);
            _mockFactory.Verify(f => f.Create(), Times.Once);
        }

        [Test]
        public void ShouldThrowErrorIfStartingUowWhenAlreadyStarted()
        {
            UnitOfWork.Start();
            var isException = false;

            try
            {
                UnitOfWork.Start();
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
            IUnitOfWork uow = UnitOfWork.Start();
            var current = UnitOfWork.Current;

            Assert.That(uow, Is.SameAs(current));
            _mockFactory.Verify(f => f.Create(), Times.Once);
        }

        [Test]
        public void ShouldThrowErrorIfAccessingCurrentUowWhenNoUowHasBeenStarted()
        {
            var isException = false;

            try
            {
                var current = UnitOfWork.Current;
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
            Assert.That(UnitOfWork.IsStarted, Is.False);

            IUnitOfWork uow = UnitOfWork.Start();
            
            Assert.That(UnitOfWork.IsStarted, Is.True);
        }

        [Test]
        public void CanGetValidCurrentSessionIfUowIsStarted()
        {
            using (UnitOfWork.Start())
            {
                ISession session = UnitOfWork.CurrentSession;
                Assert.That(session, Is.Not.Null);
                _mockFactory.VerifyAll();
            }
        }

        [TearDown]
        public void TearDownContext()
        {
            // Use reflection again to set inner uow back to null after each test
            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWork",
                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(null, null);
        }

    }
}
