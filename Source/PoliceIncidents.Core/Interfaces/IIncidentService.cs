// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Interfaces
{
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Core.Models;

    public interface IIncidentService
    {
        Task<long> CreateIncident(IncidentInputModel model);

        NewIncidentInfoModel GetIncident(long incidentId);

        Task CreateDistrict(string channelId, string channelName, string conversationId);

        DistrictEntity GetDistricForIncident(long incidentId);
    }
}