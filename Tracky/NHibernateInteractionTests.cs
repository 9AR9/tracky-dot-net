using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Tracky.Domain.EF.Music;
using Tracky.Orm;

namespace Tracky
{
    [TestFixture]
    public class NHibernateInteractionTests
    {
        // The same issue is observed here as in UnitOfWorkFactoryTests.cs - an error stating that
        // the db cannot be opened and already exists is thrown when a session creation is attempted
        // when running from a test.
        // ---
        // Is there any way to configure NHibernate to work with LocalDB databases from a test?
        // This is the same code that is in SillyStarterStuff.cs, which runs fine on Application_Start
        // and does, indeed, write to the database.
        [Test]
        [Ignore("until the database access issue is worked out")]
        public void ShouldInteractWithLocalDbEntityFrameworkDatabaseFromTestTheSameAsItDoesFromRunningApp()
        {
            var session = NHibernateHelper.GetCurrentSession();
            var transaction = session.BeginTransaction();

            var newArtist = new Artist { Name = "Rock Musician " + new Random().Next(0, 90000000) };

            session.Save(newArtist);
            transaction.Commit();

            NHibernateHelper.CloseSession();
        }
    }
}