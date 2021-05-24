// <copyright file="IncidentTeamMemberEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    using System;

    public class IncidentTeamMemberEntity
    {
        public long Id { get; set; }

        public Guid TeamMemberId { get; set; }

        public long IncidentId { get; set; }

        public UserEntity TeamMember { get; set; }

        public IncidentDetailsEntity Incident { get; set; }
    }
}
