using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Optimization;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace CorriAndMike
{   
    public class MvcApplication : System.Web.HttpApplication
    {
        public static DocumentStore Store;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);   
         
            InitializeRaven();
        }

        private static void InitializeRaven()
        {
            Store = new DocumentStore() { ConnectionStringName = "RavenDB" };
            Store.Initialize();

            //IndexCreation.CreateIndexes(typeof(NonAttendingGuestCount).Assembly, Store);
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);
        }
    }
}