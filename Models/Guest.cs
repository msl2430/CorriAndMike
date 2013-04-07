using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorriAndMike.Models
{
    public class Guest
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Type { get; set; }

        public IList<string> Invitations { get; set; } 

        public class GuestTypes
        {
            public const string Guest = "guest";
            public const string Admin = "admin";
        }
    }
}