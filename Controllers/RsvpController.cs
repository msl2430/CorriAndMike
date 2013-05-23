﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.Services;
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
            SendRsvpEmailNotification(yesGuests, invitation);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private void SendRsvpEmailNotification(List<string> guests, Invitation invitation)
        {
            var email = new MailMessage { From = new MailAddress("do-not-reply@corriandmike.com", "CorriAndMike.com") };
            var bodyContent = new StringBuilder();
            var guestNames = new List<string>();
            foreach (var guestId in guests)
            {
                var g = RavenHelper.CurrentSession().Load<Guest>(guestId);
                guestNames.Add(g.FirstName + " " + g.LastName);
            }
            
            email.To.Add("me@mikeslevine.com");
            //email.To.Add("corri.skinner@gmail.com");
            email.Subject = string.Format("Invitation {0} just RSVPed! ({1} attending)", invitation.InvitationId, guests.Count == 1 ? "1 guest" : guests.Count + " guests");
            email.IsBodyHtml = true;

            bodyContent.AppendFormat("Invitation {0} <br/><br/>", invitation.InvitationId);
            bodyContent.AppendFormat("Total guests invited: {0} <br/>", invitation.MaxNumberOfGuests);
            bodyContent.AppendFormat("Total guests attending: {0} <br/>", guests.Count);
            bodyContent.AppendFormat("Email provided: {0} <br/>", invitation.Email);
            bodyContent.Append("Guests attending: <br/>");
            foreach (var guest in guestNames)
            {
                bodyContent.AppendFormat("&nbsp;&nbsp;&nbsp;{0}<br/", guest);
            }
            email.Body = bodyContent.ToString();

            MailService.SendEmail(email);
        }
    }
}
