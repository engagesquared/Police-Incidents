using System;
using System.Collections.Generic;

namespace PoliceIncidents.Core.DB.Entities
{
    /// <summary>
    /// Connect incedent with AAD user by id
    /// </summary>
    public class UserEntity
    {
        public UserEntity()
        {
            this.IncidentTeamMembers = new HashSet<IncidentTeamMember>();
        }

        public long Id { get; set; }

        public Guid AadUserId { get; set; }

        public virtual ICollection<IncidentTeamMember> IncidentTeamMembers { get; set; }
    }
}
