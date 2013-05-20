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
        [HttpPost]
        public HttpResponseMessage Post(Guest model)
        {
            if (string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                if (RavenHelper.CurrentSession().Query<Guest>().Any(g => g.FirstName == model.FirstName && g.LastName == model.LastName))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }

                var newGuest = new Guest()
                                   {
                                       FirstName = model.FirstName, 
                                       LastName = model.LastName, 
                                       Type = Guest.GuestTypes.Guest,
                                       Invitations = model.Invitations ?? new List<string>() 
                                   };

                RavenHelper.CurrentSession().Store(newGuest);

                RavenHelper.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Created, newGuest);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        //[HttpPost]
        //public HttpResponseMessage AddInvitation(Guest model)
        //{
        //    try
        //    {
        //        var guest = RavenHelper.CurrentSession().Load<Guest>(model.Id);
        //        guest.Invitations.Add(model.Invitations.FirstOrDefault());

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    } catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
        //    }
        //}

        //[HttpPut]
        //public HttpResponseMessage Put(string id, string invitationId)
        //{
        //    try
        //    {
        //        var guest = RavenHelper.CurrentSession().Load<Guest>(id);
        //        guest.Invitations.Add(invitationId);

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
        //    }
        //}
    }
}
