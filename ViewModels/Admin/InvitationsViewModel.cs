using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorriAndMike.ViewModels.Admin
{
    public class InvitationsViewModel
    {
        public IList<InvitationTableViewModel> InvitationTable { get; set; }

        public int TotalInvitedGuests { get; set; }

        public int TotalAttendingGuests { get; set; }

        public int TotalNoGuests { get; set; }

        public int TotalWaitingInvitations { get; set; }
    }
}