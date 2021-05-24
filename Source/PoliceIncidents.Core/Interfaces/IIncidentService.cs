// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Interfaces
{
    using System.Threading.Tasks;
    using PoliceIncidents.Core.Models;

    public interface IIncidentService
    {
        Task<long> CreateIncident(IncidentInputModel model);

        NewIncidentInfoModel Get(long incidentId);
    }
}