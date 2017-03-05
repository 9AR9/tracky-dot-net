using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace Tracky.Orm
{
    public sealed class NHibernateHelper
    {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory SessionFactory; // This static attribute provides a singleton ISessionFactory for entire app to use

        static NHibernateHelper()
        {
            SessionFactory = new Configuration().Configure().BuildSessionFactory();
        }

        /// <summary>
        /// This public method provides either the current NHibernate session, if one is open, or a fresh session,
        /// to any code that needs to interact with the database. While ISessionFactory is threadsafe and can provide
        /// an ISession to many threads concurrently, ISession is a non-threadsafe object that represents a single
        /// unit-of-work with the database. ISessions are opened by an ISessionFactory and are closed when all work
        /// is completed.
        /// </summary>
        /// <returns>ISession</returns>
        public static ISession GetCurrentSession()
        {
            var context = HttpContext.Current;
            var currentSession = context.Items[CurrentSessionKey] as ISession;

            if (currentSession != null) return currentSession;
            currentSession = SessionFactory.OpenSession();
            context.Items[CurrentSessionKey] = currentSession;

            return currentSession;
        }

        public static void CloseSession()
        {
            var context = HttpContext.Current;
            var currentSession = context.Items[CurrentSessionKey] as ISession;

            if (currentSession == null)
            {
                // No current session
                return;
            }

            currentSession.Close();
            context.Items.Remove(CurrentSessionKey);
        }

        public static void CloseSessionFactory()
        {
            if (SessionFactory != null)
            {
                SessionFactory.Close();
            }
        }
    }

}