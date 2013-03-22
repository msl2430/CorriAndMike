using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CorriAndMike.Models;

namespace CorriAndMike.Api
{
    public class GuestController : CorriAndMikeBaseApiController
    {
        public HttpResponseMessage Post(Guest model)
        {
            if (string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if(RavenHelper.CurrentSession().Query<Guest>().Any(g => g.FirstName == model.FirstName && g.LastName == model.LastName))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            var newGuest = new Guest() {FirstName = model.FirstName, LastName = model.LastName, Type = Guest.GuestTypes.Guest};
            RavenHelper.CurrentSession().Store(newGuest);

            RavenHelper.CurrentSession().SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, newGuest);
        }

    }
}
