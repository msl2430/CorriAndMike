using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;

namespace CorriAndMike.Models
{
    public static class RavenHelper
    {
        private static readonly IDocumentSession RavenSession;
        
        static RavenHelper()
        {
            RavenSession = MvcApplication.Store.OpenSession();
        }

        public static IDocumentSession CurrentSession()
        {
            return RavenSession;
        }
    }
}