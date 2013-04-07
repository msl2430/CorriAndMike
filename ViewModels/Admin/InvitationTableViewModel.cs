using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorriAndMike.Models;

namespace CorriAndMike.ViewModels.Admin
{
    public class InvitationTableViewModel
    {
        public IList<Guest> Guests { get; set; }

        public Invitation Invitation { get; set; }
    }
}