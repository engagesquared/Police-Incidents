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
            this.Updates = new List<IncidentUpdateEntity>();
            this.Participants = new List<IncidentTeamMemberEntity>();
        }

        public long Id { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Gets or sets webEOC  ID of the incident.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets webEOC Link of the incident.
        /// </summary>
        public string ExternalLink { get; set; }

        public string PlannerLink { get; set; }

        public string Description { get; set; }

        public UserEntity Manager { get; set; }

        public Guid? ManagerId { get; set; }

        public Guid? CreatedById { get; set; }

        public IncidentStatus Status { get; set; }

        public string Location { get; set; }

        public string ChatConverstaionId { get; set; }

        public string FileReportFolderName { get; set; }

        public List<IncidentTeamMemberEntity> Participants { get; set; }

        public List<IncidentUpdateEntity> Updates { get; set; }

        public DateTime CreatedUtc { get; set; }

        public long DistrictId { get; set; }

        public DistrictEntity District { get; set; }
    }
}
