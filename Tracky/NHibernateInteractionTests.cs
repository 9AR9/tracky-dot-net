using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Tracky.Domain.EF.Music;
using Tracky.Orm;
using Tracky.Orm.NH;

namespace Tracky
{
    public class Context
    {
        public HttpContext FakeHttpContext(Dictionary<string, object> sessionVariables, string path)
        {
            var httpRequest = new HttpRequest(string.Empty, path, string.Empty);
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);
            httpContext.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false);

            foreach (var var in sessionVariables)
            {
                sessionContainer.Add(var.Key, var.Value);
            }

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
            return httpContext;
        }
    }

    [TestFixture]
    public class NHibernateInteractionTests
    {
        // HttpContext.Current will be null when running this test.
        // In order to run this same code, which will work in the context of a running application,
        // the HttpContext would need to be created somehow.
        [Test]
        //[Ignore("until the database access issue is worked out")]
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