// <copyright file="IncidentDetailsEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    using System;
    using System.Collections.Generic;

    public class IncidentDetailsEntity
    {
        public IncidentDetailsEntity()
        {
            this.IncidentUpdates = new List<IncidentUpdateEntity>();
            this.Participants = new List<IncidentTeamMemberEntity>();
        }

        public long Id { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Gets or sets webEOC  ID of the incident.
        /// </summary>
        public string IncidentLegacyId { get; set; }

        public string Description { get; set; }

        public string WebEOCLink { get; set; }

        public UserEntity IncidentManager { get; set; }

        public Guid? IncidentManagerId { get; set; }

        public IncidentStatus Status { get; set; }

        public string Location { get; set; }

        public string ThreadLink { get; set; }

        public List<IncidentTeamMemberEntity> Participants { get; set; }

        public List<IncidentUpdateEntity> IncidentUpdates { get; set; }

        public DateTime IncidentRaised { get; set; }

        public long DistrictId { get; set; }

        public DistrictEntity District { get; set; }
    }
}
