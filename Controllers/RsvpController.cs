using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.ViewModels.Rsvp;


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
        public ActionResult GetInvitation(string invitationId)
        {
            var model = new RsvpInvitationViewModel();
            var invitation = RavenHelper.CurrentSession().Query<Invitation>().SingleOrDefault(i => i.InvitationId == invitationId);
            if (invitation != null)
            {
                model.Invitation = invitation;
                model.InvitationGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Any(id => id == invitation.Id)).ToList();
            }
            else
            {
                TempData["LoginError"] = "There was an error retrieving your invitation.";
                return PartialView("_Login");
            }

            return PartialView("_Invitation", model);
        }

        [HttpPost]
        public HttpResponseMessage SubmitInvitation(string invitationId, string email, string attendingGuests)
        {
            var yesGuests = attendingGuests.Split(',').Any() ? attendingGuests.Split(',').ToList() : new List<string>();
            var invitation = RavenHelper.CurrentSession().Query<Invitation>().SingleOrDefault(i => i.InvitationId == invitationId);
            if (invitationId != null)
            {
                invitation.Email = email;
                invitation.RsvpDate = DateTime.Now;
                invitation.AttendingGuests.Clear();
                foreach (var guest in yesGuests)
                {
                    invitation.AttendingGuests.Add(guest);
                }
            }
            RavenHelper.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
