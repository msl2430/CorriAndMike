using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorriAndMike.Models;

namespace CorriAndMike.ViewModels.Rsvp
{
    public class RsvpInvitationViewModel
    {
        public Invitation Invitation { get; set; }

        public List<Guest> InvitationGuests { get; set; } 
    }
}