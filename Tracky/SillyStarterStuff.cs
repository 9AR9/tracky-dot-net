using System;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.FromNHSite;

namespace Tracky
{
    public class SillyStarterStuff
    {
        public static void ProveNHibernateInteraction()
        {
            /* 
             * This bit of code represents the simplest example of persisting data to the database via
             * NHibernate. If the call to it in Application_Start of Global.asax.cs were to be uncommented,
             * a new Artist entry would be created in the database immediately after the application is started.
             * 
             * This code is only here to demonstrate that NHibernate has been successfully implemented
             * into this project, and that it can persist to a database that was created via Entity Framework
             * and EF's code-first approach. It utilizes a simple "NHibernate helper" object sourced mostly
             * from NHibernate's old online tutorials, but is NOT meant to be used as live code. This solution
             * instead uses a more robust implementation of the Repository and Unit of Work patterns, with
             * full test support.
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