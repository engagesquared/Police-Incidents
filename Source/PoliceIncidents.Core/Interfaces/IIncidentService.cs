// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Core.Models;

    public interface IIncidentService
    {
        Task<long> CreateIncident(IncidentInputModel model);

        Task UpdateIncidentConversationId(long incidentId, string conversationId);

        NewIncidentInfoModel GetIncident(long incidentId);

        Task CreateDistrict(Guid groupId, string teamName, string conversationId);

        DistrictEntity GetDistricForIncident(long incidentId);
    }
}