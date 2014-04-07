using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorriAndMike.Controllers
{
    public class AboutController : CorriAndMikeBaseController
    {
        [HttpPost]
        public ActionResult Wedding()
        {
            return PartialView("_Wedding");
        }

        [HttpPost]
        public ActionResult CorriAndMike()
        {
            return PartialView("_CorriAndMike");
        }

        [HttpPost]
        public ActionResult AfterParty()
        {
            return PartialView("_AfterParty");
        }
    }
}
