using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.Services;
using CorriAndMike.ViewModels.Admin;
using Raven.Client;

namespace CorriAndMike.Controllers
{
    public class AdminController : CorriAndMikeBaseController
    {
        public ActionResult Invitations()
        {            
            var stats = new RavenQueryStatistics();
            var model = new InvitationsViewModel()
                {
                    InvitationTable = GetInvitationTableModel(),
                    TotalInvitedGuests = RavenHelper.CurrentSession().Query<Guest>().Customize(x => x.WaitForNonStaleResults()).Statistics(out stats).Count(),
                    TotalAttendingGuests = RavenHelper.CurrentSession().Query<Invitation>().Select(i => new {i.InvitationId, i.AttendingGuests.Count}).ToList().Sum(x => x.Count),
                    TotalNoGuests = RavenHelper.CurrentSession().Query<Invitation>().Where(i => !string.IsNullOrEmpty(i.Email) && i.RsvpDate != null).Select(i => new { i.InvitationId, i.AttendingGuests.Count, i.MaxNumberOfGuests}).ToList().Sum(i => i.MaxNumberOfGuests - i.Count),
                    TotalWaitingInvitations = RavenHelper.CurrentSession().Query<Invitation>().Count(i => string.IsNullOrEmpty(i.Email) && i.RsvpDate == null)
                };

            return View(model);
        }

        public ActionResult ExportEmailAddresses()
        {
            Response.Clear();
            Response.AddHeader("Content-Type", "application/ms-excel");
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"CorriAndMike-EmailAddresses.xls\";"));

            var invites = RavenHelper.CurrentSession().Query<Invitation>().Customize(x => x.Include<Guest>(g => g.Id)).Where(i => i.Email != null && i.AttendingGuests.Any()).ToList();
            var guests = new List<Guest>();
            var model = new EmailAddresses() { Addresses = new List<AddressPair>() };
            foreach (var invite in invites)
            {
                foreach (var temp in invite.AttendingGuests)
                {
                    guests.Add(RavenHelper.CurrentSession().Load<Guest>(temp));
                }
                model.Addresses.Add(new AddressPair()
                    {
                        InvitationId = invite.InvitationId,
                        Email = invite.Email,
                        Guests = guests.Select(g => string.Concat(g.FirstName, " ", g.LastName)).ToList()
                    });
                guests.Clear();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult GetAddInvitationModel()
        {
            var model = new AddInvitationViewModel()
                            {
                                Invitation = new Invitation(),
                                AvailableGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Count == 0).ToList(),
                                InvitedGuests = new List<Guest>()
                            };

            return PartialView("_AddInvitation", model);
        }

        [HttpPost]
        public ActionResult GetInvitationModel(string invitationId)
        {
            var model = new AddInvitationViewModel()
                {
                    Invitation = RavenHelper.CurrentSession().Load<Invitation>(invitationId),
                    AvailableGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Count == 0).ToList(),
                    InvitedGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Any(id => id == invitationId)).ToList()
                };

            return PartialView("_AddInvitation", model);
        }

        [HttpPost]
        public void AddGuestToInvitation(string guestIds, string invitationId)
        {
            var udpateGuests = RavenHelper.CurrentSession().Include<Guest>(g => g.Id)
                .Load(guestIds.Split(','));
            foreach (var guest in udpateGuests)
            {
                var g = RavenHelper.CurrentSession().Load<Guest>(guest.Id);
                g.Invitations.Add(invitationId);
                RavenHelper.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult GetInvitationTable()
        {
            return PartialView("_InvitationTable", GetInvitationTableModel());
        }

        [HttpPost]
        public void UpdateInvitation(Invitation invitation, string guestIds)
        {
            var invite = RavenHelper.CurrentSession().Load<Invitation>(invitation.Id);
            var updatedGuests = RavenHelper.CurrentSession().Include<Guest>(g => g.Id).Load(guestIds.Split(','));
            var existingGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Any(i => i == invitation.Id)).ToList();
            if (invite != null)
            {
                invite.Type = invitation.Type;
                invite.MaxNumberOfGuests = invitation.MaxNumberOfGuests;
            }
            foreach (var existing in existingGuests.Where(eg => guestIds.Split(',').Contains(eg.Id) == false))
            {
                existing.Invitations.Remove(invitation.Id);
                RavenHelper.SaveChanges();
            }
            foreach (var updated in updatedGuests.Where(ug => existingGuests.Select(eg => eg.Id).Contains(ug.Id) == false))
            {
                updated.Invitations.Add(invitation.Id);
                RavenHelper.SaveChanges();
            }
        }

        public ActionResult EmailTemplate(string emailTemplate)
        {
            switch (emailTemplate.ToLower())
            {
                case "booking":
                    return View("EmailTemplates/_Booking", new EmailTemplateViewModel());
                case "hotelinfo":
                    return View("EmailTemplates/_HotelInfo", new EmailTemplateViewModel());
                case "ironmonkey":
                    return View("EmailTemplates/_IronMonkeyInfo", new EmailTemplateViewModel());
                case "afterparty":
                    return View("EmailTemplates/_AfterPartyInfo", new EmailTemplateViewModel());
                case "arrival":
                    return View("EmailTemplates/_ArrivalInformation", new EmailTemplateViewModel());
                default:
                    return View("EmailTemplates/_IronMonkeyInfo", new EmailTemplateViewModel());
            }
        }

        public ActionResult SendIronMonkeyEmails()
        {
            var invites = RavenHelper.CurrentSession().Query<Invitation>().Customize(x => x.Include<Guest>(g => g.Id)).Where(i => i.Email != null && i.AttendingGuests.Any()).ToList();
            var guests = new List<Guest>();
            var sentEmails = new List<string>();
            foreach (var invite in invites)
            {
                foreach (var temp in invite.AttendingGuests)
                {
                    guests.Add(RavenHelper.CurrentSession().Load<Guest>(temp));
                }
                var email = PrepareIronMonkeyEmail(new AddressPair()
                    {
                        InvitationId = invite.InvitationId,
                        Email = invite.Email,
                        Guests = guests.Select(g => g.FirstName).ToList()
                    });
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]) && !sentEmails.Contains(invite.Email))
                    MailService.SendEmail(email);
                
                sentEmails.Add(invite.Email);
                guests.Clear();
            }

            return RedirectToAction("Invitations", "Admin");
        }

        public ActionResult SendHotelInformationEmail()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]))
                return RedirectToAction("EmailTemplate", "Admin", new {emailTemplate = "hotelinfo"});

            foreach (var address in EmailList.Emails)
            {
                var email = new MailMessage { From = new MailAddress("do-not-reply@corriandmike.com", "CorriAndMike.com") };
                var bodyContent = new StringWriter();
                email.To.Add(address);
                email.Subject = string.Format("Corri & Mike - Wedding Hotel Information");
                email.IsBodyHtml = true;

                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "EmailTemplates/_HotelInfo");
                viewResult.View.Render(new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, bodyContent), bodyContent);

                email.Body = bodyContent.ToString();

                MailService.SendEmail(email);    
            }
            
            return RedirectToAction("EmailTemplate", "Admin", new {emailTemplate = "hotelinfo"});
        }

        public ActionResult SendBookingInformationEmail()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]))
                return RedirectToAction("EmailTemplate", "Admin", new { emailTemplate = "booking" });

            foreach (var address in EmailList.Emails)
            {
                var email = new MailMessage { From = new MailAddress("do-not-reply@corriandmike.com", "CorriAndMike.com") };
                var bodyContent = new StringWriter();
                email.To.Add(address);
                email.Subject = string.Format("Corri & Mike - Hotel Booking Information");
                email.IsBodyHtml = true;

                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "EmailTemplates/_Booking");
                viewResult.View.Render(new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, bodyContent), bodyContent);

                email.Body = bodyContent.ToString();

                MailService.SendEmail(email);
            }

            return RedirectToAction("EmailTemplate", "Admin", new { emailTemplate = "booking" });
        }

        public ActionResult SendArrivalInformationEmail()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]))
                return RedirectToAction("EmailTemplate", "Admin", new { emailTemplate = "arrival" });

            foreach (var address in EmailList.Emails)
            {
                var email = new MailMessage { From = new MailAddress("do-not-reply@corriandmike.com", "CorriAndMike.com") };
                var bodyContent = new StringWriter();
                email.To.Add(address);
                email.Subject = string.Format("Corri & Mike - St. Lucia Arrival Information");
                email.IsBodyHtml = true;

                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "EmailTemplates/_ArrivalInformation");
                viewResult.View.Render(new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, bodyContent), bodyContent);

                email.Body = bodyContent.ToString();

                MailService.SendEmail(email);
            }

            return RedirectToAction("EmailTemplate", "Admin", new { emailTemplate = "arrival" });
        }

        private MailMessage PrepareIronMonkeyEmail(AddressPair recipient) 
        {
            var email = new MailMessage { From = new MailAddress("do-not-reply@corriandmike.com", "CorriAndMike.com") };
            var bodyContent = new StringWriter();
            email.To.Add(recipient.Email);
            email.Subject = string.Format("Engagement party information");
            email.IsBodyHtml = true;

            var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "EmailTemplates/_IronMonkeyInfo");
            viewResult.View.Render(new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, bodyContent), bodyContent);

            email.Body = bodyContent.ToString();

            return email;
        }

        private IList<InvitationTableViewModel> GetInvitationTableModel()
        {
            var model = new List<InvitationTableViewModel>();
            var guestWithInvitation = RavenHelper.CurrentSession().Query<Guest>()
                                                 .Customize(g => g.Include<Invitation>(i => i.InvitationId))
                                                 .Where(g => g.Invitations != null)
                                                 .ToList();

            foreach (var guest in guestWithInvitation)
            {
                foreach (var invite in guest.Invitations)
                {
                    if (model.Any(m => m.Invitation.Id == invite))
                    {
                        model.FirstOrDefault(m => m.Invitation.Id == invite).Guests.Add(guest);
                    }
                    else
                    {
                        model.Add(new InvitationTableViewModel
                        {
                            Invitation = RavenHelper.CurrentSession().Load<Invitation>(invite),
                            Guests = new List<Guest>() { guest }
                        });
                    }
                }
            }

            return model;
        } 
    }
}
