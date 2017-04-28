using System.Data;
using Moq;
using NHibernate;
using NUnit.Framework;
using Tracky.Orm.NH.GabrielSchenker;

namespace Tracky.Tests.Unit.Orm.NH.GabrielSchenker
{
    [TestFixture]
    class UnitOfWorkImplementorTests
    {
        private Mock<IUnitOfWorkFactory> _mockFactory;
        private Mock<ISession> _mockSession;
        private Mock<ITransaction> _mockTransaction;
        private IUnitOfWorkImplementor _uow;

        [SetUp]
        public void SetUpContext()
        {
            _mockFactory = new Mock<IUnitOfWorkFactory>();
            _mockSession = new Mock<ISession>();
            _mockTransaction = new Mock<ITransaction>();
        }

        [Test]
        public void CanDisposeUnitOfWorkImplementor()
        {
            _uow = new UnitOfWorkImplementor(_mockFactory.Object, _mockSession.Object);

            _uow.Dispose();

            _mockFactory.Verify(f => f.DisposeUnitOfWork(It.IsAny<IUnitOfWorkImplementor>()), Times.Once);
            _mockSession.Verify(s => s.Dispose(), Times.Once);
        }

        [Test]
        public void CanBeginTransaction()
        {
            _uow = new UnitOfWorkImplementor(_mockFactory.Object, _mockSession.Object);

            var transaction = _uow.BeginTransaction();

            _mockSession.Verify(s => s.BeginTransaction(), Times.Once);
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void CanBeginTransactionSpecifyingIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Serializable;
            _uow = new UnitOfWorkImplementor(_mockFactory.Object, _mockSession.Object);

            var transaction = _uow.BeginTransaction(isolationLevel);

            _mockSession.Verify(s => s.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Once);
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void CanExecuteTransactionalFlush()
        {
            _mockSession.Setup(s => s.BeginTransaction(IsolationLevel.ReadCommitted)).Returns(_mockTransaction.Object);
            _uow = new UnitOfWorkImplementor(_mockFactory.Object, _mockSession.Object);

            _uow.TransactionalFlush();

            _mockTransaction.Verify(t => t.Commit(), Times.Once);
            _mockTransaction.Verify(t => t.Dispose(), Times.Once);
        }

        [Test]
        public void CanExecuteTransactionalFlushSpecifyingIsolationLevel()
        {
            _mockSession.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(_mockTransaction.Object);
            _uow = new UnitOfWorkImplementor(_mockFactory.Object, _mockSession.Object);

            _uow.TransactionalFlush(IsolationLevel.Serializable);

            _mockTransaction.Verify(t => t.Commit(), Times.Once);
            _mockTransaction.Verify(t => t.Dispose(), Times.Once);
        }
    }
}
