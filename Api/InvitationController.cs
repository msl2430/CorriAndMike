using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CorriAndMike.Models;
using Raven.Client.Linq;

namespace CorriAndMike.Api
{
    public class InvitationController : CorriAndMikeBaseApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(Invitation model)
        {
            if(model == null || model.MaxNumberOfGuests == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            //TODO Check to see if any Guest is already on an invitation
            try
            {
                var newInvitation = new Invitation()
                                        {
                                            InvitationId = model.InvitationId,
                                            Type = model.Type,
                                            Email = string.Empty,
                                            MaxNumberOfGuests = model.MaxNumberOfGuests,
                                            AttendingGuests = new List<string>(),
                                            RsvpDate = null,
                                        };

                RavenHelper.CurrentSession().Store(newInvitation);

                //var udpateGuests = RavenHelper.CurrentSession().Include<Guest>(g => g.Id)
                //    .Load(Guests);
                //foreach (var guest in udpateGuests)
                //{
                //    var g = RavenHelper.CurrentSession().Load<Guest>(guest.Id);
                //    g.Invitations.Add(newInvitation.Id);
                //}

                RavenHelper.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Created, newInvitation);
            } catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(string invitationId, string password)
        {
            if (password == ConfigurationManager.AppSettings["UniversalPassword"])
            {
                var invitation = RavenHelper.CurrentSession().Query<Invitation>().SingleOrDefault(i => i.InvitationId == invitationId);
                if (invitation != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
