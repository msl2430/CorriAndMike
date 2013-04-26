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
        public ActionResult AboutTheCouple()
        {
            return PartialView("_AboutTheCouple");
        }

    }
}
