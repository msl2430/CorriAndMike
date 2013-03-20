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
        public ActionResult AddGuest()
        {
            return View(new Guest());
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
    }
}
