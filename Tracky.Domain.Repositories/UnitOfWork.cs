using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Tracky.Domain.Repositories
{
    /// <summary>
    /// Fowler describes Unit of Work: https://martinfowler.com/eaaCatalog/unitOfWork.html
    /// "A Unit of Work keeps track of everything you do during a business transaction that
    /// can affect the database. When you're done, it figures out everything that needs to be
    /// done to alter the database as a result of your work."
    /// 
    /// This UnitOfWork class is a static wrapper for NHibernate's Session object,
    /// which is its container for a Unit of Work.
    /// 
    /// Our wrapper class includes a Start() method to create an instance of IUnitOfWork,
    /// using an IUnitOfWorkFactory, and store it internally as the "current"
    /// session, aka "Unit of Work".
    /// 
    /// </summary>
    public static class UnitOfWork // TODO: Rename to UnitOfWorkWrapper? UnitOfWorkManager? This class does not implement IUnitOfWork, so name should probably change.
    {
        private static IUnitOfWorkFactory _unitOfWorkFactory;
        private static IUnitOfWork _unitOfWork;

        public static IUnitOfWork Current
        {
            get
            {
                if(_unitOfWork == null)
                    throw new InvalidOperationException("You are not in a unit of work");
                return _unitOfWork;
            }
        }

        public static ISession CurrentSession
        {
            get { return _unitOfWorkFactory.CurrentSession; }
            internal set { _unitOfWorkFactory.CurrentSession = value; }
        }

        public static bool IsStarted
        {
            get { return _unitOfWork != null; }
        }

        //public virtual ISession Session { get; set; }
        //public virtual ISessionFactory SessionFactory { get; set; }

        //public bool IsTransactionInProgress { get { return Session.Transaction.IsActive; } }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}

        public static IUnitOfWork Start()
        {
            if (_unitOfWork != null)
                throw new InvalidOperationException("You cannot start more than one unit of work at the same time.");

            _unitOfWork = _unitOfWorkFactory.Create();
            return _unitOfWork;
        }
    }
}
