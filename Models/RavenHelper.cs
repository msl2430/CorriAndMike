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
        private static IDocumentSession _ravenSession;
        
        static RavenHelper()
        {
            _ravenSession = MvcApplication.Store.OpenSession();
        }

        public static IDocumentSession CurrentSession()
        {
            //_ravenSession.SaveChanges();
            _ravenSession = MvcApplication.Store.OpenSession();
            return _ravenSession;
        }

        public static void SaveChanges()
        {
            _ravenSession.SaveChanges();
            _ravenSession = MvcApplication.Store.OpenSession();
        }
    }
}