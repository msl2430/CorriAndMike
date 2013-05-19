using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CorriAndMike.Models;
using CorriAndMike.ViewModels.Admin;
using CorriAndMike.ViewModels.Rsvp;

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
            //var ravenUser = RavenSession.Load<Invitation>("guestgroups/1");
            //ViewBag.GuestGroup = ravenUser;
            return View();
        }

        [HttpPost]
        public ActionResult Home()
        {
            var model = new RsvpInvitationViewModel();
            const string invitationId = "CM306";
            var invitation = RavenHelper.CurrentSession().Query<Invitation>().SingleOrDefault(i => i.InvitationId == invitationId);
            if (invitation != null)
            {
                model.Invitation = invitation;
                model.InvitationGuests = RavenHelper.CurrentSession().Query<Guest>().Where(g => g.Invitations.Any(id => id == invitation.Id)).ToList();
            }

            //return PartialView("_Home", model);
            //var guestWithInvitation = RavenHelper.CurrentSession().Query<Guest>()
            //                                     .Customize(g => g.Include<Invitation>(i => i.InvitationId))
            //                                     .Where(g => g.Invitations != null)
            //                                     .ToList();

            //foreach (var guest in guestWithInvitation)
            //{
            //    foreach (var invite in guest.Invitations)
            //    {
            //        if (model.Invitation.Any(m => m.Invitation.Id == invite))
            //        {
            //            model.FirstOrDefault(m => m.Invitation.Id == invite).Guests.Add(guest);
            //        }
            //        else
            //        {
            //            model.Add(new InvitationTableViewModel
            //            {
            //                Invitation = RavenHelper.CurrentSession().Load<Invitation>(invite),
            //                Guests = new List<Guest>() { guest }
            //            });
            //        }
            //    }
            //}
            return PartialView("_Home", model);
        }
    }
}
