using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorriAndMike.Controllers
{
    public class RsvpController : CorriAndMikeBaseController
    {
        [HttpPost]
        public ActionResult Login()
        {
            return PartialView("_Login");
        }
    }
}
