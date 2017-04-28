using System;
using System.IO;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;

namespace Tracky.Orm.NH.GabrielSchenker
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private static ISession _currentSession;
        private static Configuration _configuration;
        private static ISessionFactory _sessionFactory;

        public ISession CurrentSession
        {
            get
            {
                if(_currentSession == null)
                    throw new InvalidOperationException("You are not in a unit of work.");
                return _currentSession;
            }
            set { _currentSession = value; }
        }
        public Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Configuration();
                    string hibernateConfig = "hibernate.cfg.xml";
                    // if not rooted, assume path from base directory
                    if (Path.IsPathRooted(hibernateConfig) == false)
                    {
                        hibernateConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hibernateConfig);
                    }
                    if (File.Exists(hibernateConfig))
                        _configuration.Configure(new XmlTextReader(hibernateConfig));
                }
                return _configuration;
            }
        }

        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = Configuration.BuildSessionFactory();
                return _sessionFactory;
            }
        }

        public IUnitOfWorkImplementor Create()
        {
            ISession session = CreateSession();
            session.FlushMode = FlushMode.Commit;
            _currentSession = session;
            return new UnitOfWorkImplementor(this, session);
        }

        private ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }

        public void DisposeUnitOfWork(IUnitOfWorkImplementor adapter)
        {
            CurrentSession = null;
            UnitOfWorkManager.DisposeUnitOfWork(adapter);
        }
    }
}
