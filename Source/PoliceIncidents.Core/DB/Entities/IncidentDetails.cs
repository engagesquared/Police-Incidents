using System;
using System.Collections.Generic;

namespace PoliceIncidents.Core.DB.Entities
{
    public class IncidentDetails
    {
        public IncidentDetails()
        {
            this.IncidentUpdates = new HashSet<IncidentUpdate>();
            this.Participants = new HashSet<IncidentTeamMember>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// WebEOC  ID of the incident 
        /// </summary>
        public string IncidentLegacyId { get; set; }
        public string Description { get; set; }
        public string WebEOCLink { get; set; }
        public UserEntity IncidentManager { get; set; }        
        public IncidentStatus Status { get; set; }
        public string Location { get; set; }        
        public string ThreadLink { get; set; }
        public virtual ICollection<IncidentTeamMember> Participants { get; set; }
        public virtual ICollection<IncidentUpdate> IncidentUpdates { get; set; }
        public DateTime IncidentRaised { get; set; }
    }
}
