// <copyright file="UserEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Connect incedent with AAD user by id.
    /// </summary>
    public class UserEntity
    {
        public Guid AadUserId { get; set; }

        public string BotUserId { get; set; }

        public string ConversationId { get; set; }

        public List<IncidentTeamMemberEntity> IncidentTeamMembers { get; set; }

        public List<IncidentDetailsEntity> IncidentsManagedByUser { get; set; }

        public List<IncidentUpdateEntity> IncidentUpdatesCreatedByUser { get; set; }
    }
}
