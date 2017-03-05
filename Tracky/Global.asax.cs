using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Tracky
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            // Added this call as a temporary, simple route to proving NHibernate is properly installed and configured to persist to db created via Entity Framework code-first.
            // Uncomment to test at application startup.
            //SillyStarterStuff.ProveNHibernateInteraction();
            

            // Added this ignore of ReferenceLoopHandling to allow API controllers to serialize and return referenced types for JSON
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;


            // Added this removal of XML formatting until a proper solution to serialization of proxies for API calls is found
            // (Need to look into DataContractResolver for DataContractSerializer)
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
    }
}