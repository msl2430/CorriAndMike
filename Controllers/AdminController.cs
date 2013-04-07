using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.ViewModels;
using CorriAndMike.ViewModels.Admin;

namespace CorriAndMike.Controllers
{
    public class AdminController : CorriAndMikeBaseController
    {
        public ActionResult Test()
        {
            var guestWithInvitation = RavenHelper.CurrentSession().Query<Guest>()
                                                 .Customize(g => g.Include<Invitation>(i => i.InvitationId))
                                                 .Where(g => g.Invitations != null)
                                                 .ToList();

            foreach (var guest in guestWithInvitation)
            {
                foreach (var invite in guest.Invitations)
                {
                    var test = RavenHelper.CurrentSession().Load<Invitation>(invite);
                }
            }      

            return View();
        }

        public ActionResult AddInvitation()
        {
            var model = new AddInvitationViewModel()
                            {
                                Invitation = new Invitation(),
                                AvailableGuests = RavenSession.Query<Guest>().Where(g => g.Invitations == null).ToList(),
                            };
            return View(model);
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
                RavenHelper.CurrentSession().SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult GetInvitationTable()
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
                                Guests = new List<Guest>() {guest}
                            });
                    }
                }
            }

            return PartialView("_InvitationTable", model);
        }
    }
}
