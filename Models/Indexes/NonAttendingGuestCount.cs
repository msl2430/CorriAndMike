using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;

namespace CorriAndMike.Models.Indexes
{
    public class NonAttendingGuestCount : AbstractIndexCreationTask<Invitation, NonAttendingGuestCount.ReduceResult>
    {
        public class ReduceResult
        {
            public string Id { get; set; }
            public int MaxNumberOfGuests { get; set; }
            public int AttendingCount { get; set; }
        }

        public NonAttendingGuestCount()
        {
            Map = invites => from i in invites
                             where i.MaxNumberOfGuests != i.AttendingGuests.Count
                             select new
                                 {
                                     Id = i.Id,
                                     MaxNumberOfGuests = i.MaxNumberOfGuests,
                                     AttendingCount = i.AttendingGuests.Select(x => x).Count()
                                 };

            TransformResults = (database, invites) => from i in invites
                                                      select new { Id = i.Id, MaxNumberOfGuests = i.MaxNumberOfGuests, AttendingCount = i.AttendingCount};
        }
    }
}