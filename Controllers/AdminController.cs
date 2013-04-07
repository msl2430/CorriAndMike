using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.ViewModels;

namespace CorriAndMike.Controllers
{
    public class AdminController : CorriAndMikeBaseController
    {
        public ActionResult Test()
        {
            return View();
        }

        public ActionResult AddInvitation()
        {
            var model = new AddInvitationViewModel()
                            {
                                Invitation = new Invitation(),
                                AvailableGuests = RavenSession.Query<Guest>().ToList(),
                            };
            return View(model);
        }

        [HttpPost]
        public void AddGuestToInvitation(IList<string> guestIds, string invitationId)
        {
            var udpateGuests = RavenHelper.CurrentSession().Include<Guest>(g => g.Id)
                .Load(guestIds);
            foreach (var guest in udpateGuests)
            {
                var g = RavenHelper.CurrentSession().Load<Guest>(guest.Id);
                g.Invitations.Add(invitationId);
            }
        }
    }
}
