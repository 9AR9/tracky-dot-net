using System;
using System.Data;
using NHibernate;

namespace Tracky.Orm.NH.GabrielSchenker
{
    // Schenker originally called this IUnitOfWork in the beginning of his tutorial,
    // but by the end of Part 2, in the UML, he uses both IUnitOfWork and
    // IUnitOfWorkImplementor. To maintain synergy between the interface and the
    // concrete implementation, I have renamed this to IUnitOfWorkImplementor.
    // References to "IUnitOfWork" in his tutorial are references to this interface.
    public interface IUnitOfWorkImplementor : IDisposable 
    {
        bool IsInActiveTransaction { get; }
        IUnitOfWorkFactory Factory { get; }
        ISession Session { get; }

        IGenericTransaction BeginTransaction();
        IGenericTransaction BeginTransaction(IsolationLevel isolationLevel);
        void TransactionalFlush();
        void TransactionalFlush(IsolationLevel serializable);
    }
}
