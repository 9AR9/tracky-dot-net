using System;
using Tracky.Domain.EF.Music;
using Tracky.Orm;
using Tracky.Orm.NH;

namespace Tracky
{
    public class SillyStarterStuff
    {
        public static void ProveNHibernateInteraction()
        {
            /* This measly bit of code represents the simplest example of persisting data to the database
             * via NHibernate. If the call to it in Application_Start of Global.asax.cs were to be uncommented,
             * it would create a new Artist entry in the database immediately after the application is started.
             * 
             * This code is only here to demonstrate that NHibernate has been successfully implemented
             * into this project, and that it can persist to a database that was created via
             * Enitity Framework and its code-first approach. It should be removed once solid
             * Repository and Unit of Work patterns can be implemented, so that Repositories are
             * available to perform CRUD for all domain objects, with integration tests proving
             * their behavior. These integration tests MUST be sure to make changes in-memory while
             * tests are run, and to discard changes after the tests finish, in the same way our
             * Entity Framework integration tests do.
             */

            var session = NHibernateHelper.GetCurrentSession();
            var transaction = session.BeginTransaction();

            var newArtist = new Artist { Name = "Rock Musician " + new Random().Next(0, 90000000) };

            session.Save(newArtist);
            transaction.Commit();

            NHibernateHelper.CloseSession();
        }
    }
}