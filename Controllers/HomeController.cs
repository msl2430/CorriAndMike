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
            return View();
        }

        [HttpPost]
        public ActionResult Home()
        {
            return PartialView("_Home");
        }
    }
}
