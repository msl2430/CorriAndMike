using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorriAndMike.Controllers
{
    public class EventsController : CorriAndMikeBaseController
    {
        [HttpPost]
        public ActionResult EngagementParty()
        {
            return View("_EngagementParty");
        }
    }
}
