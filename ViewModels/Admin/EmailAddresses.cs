using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorriAndMike.ViewModels.Admin
{
    public class EmailAddresses
    {
        public IList<AddressPair> Addresses { get; set; } 
    }

    public class AddressPair
    {
        public string InvitationId { get; set; }
        public IList<string> Guests { get; set; } 
        public string Email { get; set; }
    }
}