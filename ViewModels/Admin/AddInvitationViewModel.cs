using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CorriAndMike.Models;

namespace CorriAndMike.ViewModels.Admin
{
    public class AddInvitationViewModel
    {
        public Invitation Invitation { get; set; }

        public List<Guest> AvailableGuests { get; set; }

        public List<Guest> InvitedGuests { get; set; } 

        public IEnumerable<SelectListItem> InvitationSelectType
        {
            get
            {
                return new List<SelectListItem>()
                           {
                               new SelectListItem() { Selected = false, Text = "Engagement Party", Value = Convert.ToString((int)Invitation.InvitationType.EngagementParty)},
                               new SelectListItem() { Selected = false, Text = "Wedding", Value = Convert.ToString((int)Invitation.InvitationType.Wedding)}
                           };
            }
        } 
    }
}