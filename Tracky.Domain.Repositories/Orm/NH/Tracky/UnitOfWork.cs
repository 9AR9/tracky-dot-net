using System;
using NHibernate;

namespace Tracky.Domain.Repositories.Orm.NH.Tracky
{
    public class UnitOfWork : IDisposable
    {
        private ISessionFactory SessionFactory { get; set; }
        public ISession Session { get; set; }

        public bool IsTransactionInProgress => Session.Transaction.IsActive;

        public UnitOfWork(bool forTesting)
        {
            SessionFactory = NHibernateHelper.GetSessionFactory(forTesting);
            Session = NHibernateHelper.OpenSession(forTesting);
        }

        public void BeginTransaction()
        {
            if (!IsTransactionInProgress)
            {
                Session.BeginTransaction();
            }
        }
        public void FinishTransaction(bool success)
        {
            if (success)
            {
                CommitTransaction();
            }
            else
            {
                RollbackTransaction();
            }
        }

        protected void CommitTransaction()
        {
            if (IsTransactionInProgress)
            {
                Session.Transaction.Commit();
            }
        }

        protected void RollbackTransaction()
        {
            if (IsTransactionInProgress)
            {
                Session.Transaction.Rollback();
            }
        }

        public void FlushAndClear()
        {
            if (IsTransactionInProgress)
            {
                Session.Flush();
                Session.Clear();
            }
        }

        public void Dispose()
        {
            Session.Close();
            Session.Dispose();
        }
    }
}