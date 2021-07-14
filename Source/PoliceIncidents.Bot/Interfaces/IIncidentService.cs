// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;

    public interface IIncidentService
    {

        Task UpdateIncidentChatMessageId(long incidentId, string messageId);

        IncidentDetailsEntity GetIncident(long incidentId);

        Task EnsureDistrict(Guid groupId, string teamName, string conversationId);

        List<IncidentTeamMemberEntity> GetIncidentTeamMembers(long incidentId);
    }
}