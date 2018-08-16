using NUnit.Framework;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Tests.Integration
{
    public abstract class NhDatabaseIntegrationTestBase
    {
        public UnitOfWork UnitOfWork;

        [SetUp]
        public void SetUpBase()
        {
            UnitOfWork = new UnitOfWork(true);
            UnitOfWork.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            UnitOfWork.FinishTransaction(false); // change argument to true to allow test data to persist to database
            UnitOfWork.Session.Close();
        }
    }
}
