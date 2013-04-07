using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace CorriAndMike.Models
{
    public class Invitation
    {
        public string Id { get; set; }

        public string InvitationId { get; internal set; }

        public int Type { get; set; }

        public string Password { get { return "corri&mike"; } }

        public string Email { get; set; }

        public int MaxNumberOfGuests { get; set; }

        public int AttendingGuests { get; set; }

        public DateTime? RsvpDate { get; set; }

        public enum InvitationType
        {
            EngagementParty,
            Wedding
        }

        public Invitation()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            InvitationId = string.Concat("CM", rand.Next(9), rand.Next(9), rand.Next(9));
        }
    }
}