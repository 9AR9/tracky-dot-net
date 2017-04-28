using System;
using System.Data;
using NHibernate;

namespace Tracky.Orm.NH.GabrielSchenker
{
    public class UnitOfWorkImplementor : IUnitOfWorkImplementor
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISession _session;

        public bool IsInActiveTransaction { get { return _session.Transaction.IsActive; } }
        public IUnitOfWorkFactory Factory { get { return _unitOfWorkFactory; } }
        public ISession Session { get { return _session; } }

        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            _unitOfWorkFactory = factory;
            _session = session;
        }

        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(_session.BeginTransaction());
        }

        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(_session.BeginTransaction(isolationLevel));
        }

        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            IGenericTransaction transaction = BeginTransaction(isolationLevel);
            try
            {
                // Forces a flush of the current unit of work
                transaction.Commit();
            }
            catch (Exception)
            {
                // If the commit of the transaction fails, the transaction is rolled back.
                // In this case, we do not want to try to reuse the current unit of work
                // since the session is in an inconsistent state and must be closed.
                // Instead, we abandon this unit of work, allowing a new one to be started.
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public void Dispose()
        {
            _unitOfWorkFactory.DisposeUnitOfWork(this);
            _session.Dispose();
        }
    }
}
