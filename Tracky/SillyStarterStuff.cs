using System;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.FromNHSite.FromNHSite;

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
             * Repository and Unit of Work patterns have been implemented, with tests.
             */

            var session = NHibernateHelperThatYouShouldNotBeUsing.GetCurrentSession();
            var transaction = session.BeginTransaction();

            var newArtist = new Artist { Name = "Rock Musician " + new Random().Next(0, 90000000) };

            session.Save(newArtist);
            transaction.Commit();

            NHibernateHelperThatYouShouldNotBeUsing.CloseSession();
        }
    }
}