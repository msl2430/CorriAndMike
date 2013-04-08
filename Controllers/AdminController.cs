﻿using System;
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

        public ActionResult Invitations()
        {
            var model = new InvitationsViewModel()
                {
                    InvitationTable = GetInvitationTableModel(),
                };

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
