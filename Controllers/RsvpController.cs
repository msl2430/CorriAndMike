using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;


namespace CorriAndMike.Controllers
{
    public class RsvpController : CorriAndMikeBaseController
    {
        [HttpPost]
        public ActionResult Login()
        {
            return PartialView("_Login");
        }

        [HttpPost]
        public HttpResponseMessage RsvpLogin(string invitationId, string password)
        {
            if (password == ConfigurationManager.AppSettings["UniversalPassword"])
            {
                var invitation = RavenHelper.CurrentSession().Query<Invitation>().SingleOrDefault(i => i.InvitationId == invitationId);
                if (invitation != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                } 
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
