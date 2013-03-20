using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;
using Raven.Client.Connection;
using Raven.Client.Document;

namespace CorriAndMike.Controllers
{
    public class HomeController : CorriAndMikeBaseController
    {
        public ActionResult Index()
        {
            //var me = RavenSession.Load<User>("users/33");
            //var guestGroup = new GuestGroup()
            //                     {
            //                         Guests = new List<User>()
            //                                      {
            //                                          {me},
            //                                          new User()
            //                                              {
            //                                                  FirstName = "Corri",
            //                                                  LastName = "Skinner",
            //                                                  Type = "guest"
            //                                              }
            //                                      }
            //                     };
            //RavenSession.Store(guestGroup);
            //RavenSession.SaveChanges();
            var ravenUser = RavenSession.Load<Invitation>("guestgroups/1");
            ViewBag.GuestGroup = ravenUser;
            return View();
        }

    }
}
