using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Web.Optimization;
using System.Web.Routing;
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

            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);
            IndexCreation.CreateIndexes(typeof(AbstractIndexCreationTask).Assembly, Store);
        }
    }
}