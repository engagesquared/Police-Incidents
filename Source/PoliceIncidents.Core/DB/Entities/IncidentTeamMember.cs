using System;
using System.Collections.Generic;

namespace PoliceIncidents.Core.DB.Entities
{
    /// <summary>
    /// Connect incedent with AAD user by id
    /// </summary>
    public class IncidentTeamMember
    {
        public IncidentTeamMember()
        {
            this.Incedents = new HashSet<IncidentDetails>();
        }

        public long Id { get; set; }

        public Guid AadUserId { get; set; }

        public virtual ICollection<IncidentDetails> Incedents { get; set; }
    }
}
