namespace PoliceIncidents.Core.DB.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Connect incedent with AAD user by id.
    /// </summary>
    public class UserEntity
    {
        public UserEntity()
        {
            this.IncidentTeamMembers = new HashSet<IncidentTeamMember>();
        }

        public long Id { get; set; }

        public Guid AadUserId { get; set; }

        public string BotUserId { get; set; }

        public string ConversationId { get; set; }

        public virtual ICollection<IncidentTeamMember> IncidentTeamMembers { get; set; }
    }
}
