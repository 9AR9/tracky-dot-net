using System;
using NHibernate;
using NHibernate.Cfg;

namespace Tracky.Orm.NH.GabrielSchenker
{
    /// <summary>
    /// Fowler describes Unit of Work: https://martinfowler.com/eaaCatalog/unitOfWork.html
    /// "A Unit of Work keeps track of everything you do during a business transaction that
    /// can affect the database. When you're done, it figures out everything that needs to be
    /// done to alter the database as a result of your work."
    /// 
    /// This UnitOfWorkManager class is a static wrapper for NHibernate's Session object,
    /// which is its container for a Unit of Work.
    /// 
    /// Our wrapper class includes a Start() method to create an instance of IUnitOfWorkImplementor,
    /// using an IUnitOfWorkFactory, and store it internally as the "current"
    /// session, aka "Unit of Work".
    /// 
    /// NOTE: Gabriel Schenker calls this object merely UnitOfWork in his tutorial, but
    /// I have renamed it to UnitOfWorkManager to avoid confusion with his IUnitOfWork
    /// interface, which is actually meant for the UnitOfWorkImplementor objects, not
    /// this manager. Thusly, IUnitOfWork has also been renamed to IUnitOfWorkImplementor,
    /// which may have been Schenker's intention all along, though the tutorial actually
    /// shows both names at different times, confusingly.
    /// 
    /// </summary>
    public static class UnitOfWorkManager
    {
        private static IUnitOfWorkFactory _unitOfWorkFactory;
        private static IUnitOfWorkImplementor _unitOfWorkImplementor;

        public static IUnitOfWorkImplementor Current
        {
            get
            {
                if(_unitOfWorkImplementor == null)
                    throw new InvalidOperationException("You are not in a unit of work");
                return _unitOfWorkImplementor;
            }
        }

        public static ISession CurrentSession
        {
            get { return _unitOfWorkFactory.CurrentSession; }
            internal set { _unitOfWorkFactory.CurrentSession = value; }
        }

        public static Configuration Configuration
        {
            get { return _unitOfWorkFactory.Configuration; }
        }

        public static bool IsStarted
        {
            get { return _unitOfWorkImplementor != null; }
        }

        public static IUnitOfWorkFactory Factory
        {
            set { _unitOfWorkFactory = value; }
        }

        public static IUnitOfWorkImplementor Start()
        {
            if (_unitOfWorkImplementor != null)
                throw new InvalidOperationException("You cannot start more than one unit of work at the same time.");

            _unitOfWorkImplementor = _unitOfWorkFactory.Create();
            return _unitOfWorkImplementor;
        }

        public static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWorkImplementor)
        {
            unitOfWorkImplementor.Dispose(); // TODO: Not sure this is right, but the walkthrough doesn't specify what this should do, or why, just that we call it from the factory's DUOW method
        }
    }
}
