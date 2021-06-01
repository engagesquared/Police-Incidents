// <copyright file="IIncidentUpdateService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PoliceIncidents.Tab.Models;

    public interface IIncidentUpdateService
    {
        Task<IncidentUpdateModel> AddIncidentUpdate(IncidentUpdateInputModel incidentUpdate);

        Task<List<IncidentUpdateModel>> GetIncidentUpdates(long incidentId);
    }
}